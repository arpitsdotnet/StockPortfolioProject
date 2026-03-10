using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;

public sealed record DeleteSharePriceRequest(int SharePriceHistoryId);
public sealed record DeleteSharePriceResponse(int SharePriceHistoryId, bool IsDeleted);

public class DeleteSharePriceHandler
{
    private readonly ApplicationDbContext _context;

    public DeleteSharePriceHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DeleteSharePriceResponse>> Handle(DeleteSharePriceRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SharePriceHistoryId <= 0)
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request.");

        var entity = await _context.SharePriceHistories.FirstOrDefaultAsync(x => x.SharePriceHistoryId == request.SharePriceHistoryId, cancellationToken);
        if (entity == null)
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Share price record not found.");

        if (!entity.IsActive)
            return Result<DeleteSharePriceResponse>.Success(new DeleteSharePriceResponse(request.SharePriceHistoryId, true), "Share price already inactive.");

        entity.IsActive = false;
        entity.LastModifiedOn = DateTime.UtcNow;
        entity.LastModifiedById = 0;

        _context.SharePriceHistories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<DeleteSharePriceResponse>.Success(new DeleteSharePriceResponse(request.SharePriceHistoryId, true), "Share price deleted.");
    }
}
