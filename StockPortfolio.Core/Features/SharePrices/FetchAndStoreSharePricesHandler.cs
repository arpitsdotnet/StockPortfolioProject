using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

namespace StockPortfolio.Core.Features.SharePrices;

public sealed record FetchAndStoreSharePricesRequest(int SecurityId, DateTime FromDate, DateTime ToDate);

public class FetchAndStoreSharePricesHandler
{
    private readonly ApplicationDbContext _context;
    private readonly TimeSeriesDailyHandler _timeSeriesDailyHandler;

    public FetchAndStoreSharePricesHandler(ApplicationDbContext context, TimeSeriesDailyHandler timeSeriesDailyHandler)
    {
        _context = context;
        _timeSeriesDailyHandler = timeSeriesDailyHandler;
    }

    public async Task<ResultDto<int>> Handle(FetchAndStoreSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SecurityId <= 0)
            return new ResultDto<int> { IsSuccess = false, Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request") };

        var security = await _context.Securities.FirstOrDefaultAsync(s => s.SecurityId == request.SecurityId, cancellationToken);
        if (security == null)
            return new ResultDto<int> { IsSuccess = false, Error = new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found.") };

        // Fetch daily series via handler
        var apiResult = await _timeSeriesDailyHandler.Handle(new TimeSeriesDailyRequest(security.Symbol!), cancellationToken);
        if (apiResult.IsFailure)
            return new ResultDto<int> { IsSuccess = false, Error = apiResult.Error };

        var items = apiResult.Value;

        // Filter by date range
        var filtered = items
            .Select(i=>
            {
                if (!DateTime.TryParse(i.SeriesDateTime, out var dt)) return (Parsed: false, Date: default(DateTime), Item: i);
                return (Parsed: true, Date: dt.Date, Item: i);
            })
            .Where(p => p.Parsed && p.Date >= request.FromDate.Date && p.Date <= request.ToDate.Date)
            .ToList();

        if (!filtered.Any())
            return new ResultDto<int> { IsSuccess = false, Error = new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "No price data found for date range.") };

        var dates = filtered.Select(f => f.Date).Distinct().ToList();
        var existingHistories = await _context.SharePriceHistories
            .Where(x => x.SecurityId == request.SecurityId && dates.Contains(x.SeriesDate))
            .ToListAsync(cancellationToken);
        var existingByDate = existingHistories.ToDictionary(h => h.SeriesDate, h => h);

        // Map to domain and upsert (avoid duplicates)
        int inserted = 0;
        foreach (var f in filtered)
        {
            var apiItem = f.Item;
            var seriesDate = f.Date;

            if (existingByDate.TryGetValue(seriesDate, out var exists))
            {
                exists.Open = apiItem.Open;
                exists.High = apiItem.High;
                exists.Low = apiItem.Low;
                exists.Close = apiItem.Close;
                exists.Volume = apiItem.Volumne;
                exists.LastModifiedOn = DateTime.UtcNow;
                _context.SharePriceHistories.Update(exists);
            }
            else
            {
                var entity = new SharePriceHistory
                {
                    SecurityId = request.SecurityId,
                    SeriesDate = seriesDate,
                    Open = apiItem.Open,
                    High = apiItem.High,
                    Low = apiItem.Low,
                    Close = apiItem.Close,
                    Volume = apiItem.Volumne,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedById = 0,
                    LastModifiedOn = DateTime.UtcNow,
                    LastModifiedById = 0
                };
                await _context.SharePriceHistories.AddAsync(entity, cancellationToken);
                inserted++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ResultDto<int> { IsSuccess = true, Value = inserted, Message = $"Inserted/Updated {inserted} records." };
    }
}
