using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;

public sealed record GetLatestSharePriceRequest(int SecurityId);
public sealed record GetLatestSharePriceResponse(int SharePriceHistoryId, DateTime SeriesDate, decimal Open, decimal High, decimal Low, decimal Close, long Volume);

public class GetLatestSharePriceHandler
{
    private readonly ApplicationDbContext _context;

    public GetLatestSharePriceHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetLatestSharePriceResponse>> Handle(GetLatestSharePriceRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SecurityId <= 0)
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request.");

        var latest = await _context.SharePriceHistories
            .Where(x => x.IsActive && x.SecurityId == request.SecurityId)
            .OrderByDescending(x => x.SeriesDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest == null)
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "No share price history found for the security.");

        var resp = new GetLatestSharePriceResponse(latest.SharePriceHistoryId, latest.SeriesDate, latest.Open, latest.High, latest.Low, latest.Close, latest.Volume);
        return Result<GetLatestSharePriceResponse>.Success(resp);
    }
}
