using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.SharePrices.GetSharePrices;

public sealed record GetSharePricesRequest(int SecurityId, DateTime? FromDate = null, DateTime? ToDate = null);
public sealed record GetSharePricesResponse(int SharePriceHistoryId, DateTime SeriesDate, decimal Open, decimal High, decimal Low, decimal Close, long Volume);

public class GetSharePricesHandler
{
    private readonly ApplicationDbContext _context;

    public GetSharePricesHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<GetSharePricesResponse>>> Handle(GetSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SecurityId <= 0)
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request.");

        var securityExists = await _context.Securities.AnyAsync(s => s.SecurityId == request.SecurityId && s.IsActive, cancellationToken);
        if (!securityExists)
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found.");

        var query = _context.SharePriceHistories
            .Where(x => x.IsActive && x.SecurityId == request.SecurityId);

        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            query = query.Where(x => x.SeriesDate >= from);
        }
        if (request.ToDate.HasValue)
        {
            var to = request.ToDate.Value.Date;
            query = query.Where(x => x.SeriesDate <= to);
        }

        var list = await query.OrderBy(x => x.SeriesDate).ToListAsync(cancellationToken);

        if (list == null || list.Count == 0)
            return new Error(ErrorType.VALIDATION, ErrorCode.NOT_FOUND, "No price history found for the given criteria.");

        var response = list.Select(x => new GetSharePricesResponse(
            SharePriceHistoryId: x.SharePriceHistoryId,
            SeriesDate: x.SeriesDate,
            Open: x.Open,
            High: x.High,
            Low: x.Low,
            Close: x.Close,
            Volume: x.Volume
        )).ToList();

        return Result<List<GetSharePricesResponse>>.Success(response);
    }
}
