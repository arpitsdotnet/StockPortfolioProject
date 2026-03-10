using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.SharePrices.CreateSharePrice;

public sealed record CreateSharePriceRequest(int SecurityId, DateTime SeriesDate, decimal Open, decimal High, decimal Low, decimal Close, long Volume);
public sealed record CreateSharePriceResponse(int SharePriceHistoryId);

public class CreateSharePriceHandler
{
    private readonly ApplicationDbContext _context;

    public CreateSharePriceHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateSharePriceResponse>> Handle(CreateSharePriceRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SecurityId <= 0)
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request.");

        var securityExists = await _context.Securities.AnyAsync(s => s.SecurityId == request.SecurityId && s.IsActive, cancellationToken);
        if (!securityExists)
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found.");

        // prevent duplicate for same date
        var exists = await _context.SharePriceHistories.AnyAsync(x => x.SecurityId == request.SecurityId && x.SeriesDate == request.SeriesDate.Date, cancellationToken);
        if (exists)
            return new Error(ErrorType.FAILURE, ErrorCode.CONFLICT, "Share price for the given date already exists.");

        var entity = new SharePriceHistory
        {
            SecurityId = request.SecurityId,
            SeriesDate = request.SeriesDate.Date,
            Open = request.Open,
            High = request.High,
            Low = request.Low,
            Close = request.Close,
            Volume = request.Volume,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedById = 0,
            LastModifiedOn = DateTime.UtcNow,
            LastModifiedById = 0
        };

        await _context.SharePriceHistories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateSharePriceResponse>.Success(new CreateSharePriceResponse(entity.SharePriceHistoryId));
    }
}
