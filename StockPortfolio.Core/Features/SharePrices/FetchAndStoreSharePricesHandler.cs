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
        var filtered = items.Where(i =>
        {
            if (!DateTime.TryParse(i.SeriesDateTime, out var dt)) return false;
            var date = dt.Date;
            return date >= request.FromDate.Date && date <= request.ToDate.Date;
        }).ToList();

        if (!filtered.Any())
            return new ResultDto<int> { IsSuccess = false, Error = new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "No price data found for date range.") };

        // Map to domain and upsert (avoid duplicates)
        int inserted = 0;
        foreach (var item in filtered)
        {
            DateTime seriesDate = DateTime.Parse(item.SeriesDateTime).Date;

            var exists = await _context.SharePriceHistories.FirstOrDefaultAsync(x => x.SecurityId == request.SecurityId && x.SeriesDate == seriesDate, cancellationToken);
            if (exists != null)
            {
                exists.Open = item.Open;
                exists.High = item.High;
                exists.Low = item.Low;
                exists.Close = item.Close;
                exists.Volume = item.Volumne;
                exists.LastModifiedOn = DateTime.UtcNow;
                _context.SharePriceHistories.Update(exists);
            }
            else
            {
                var entity = new SharePriceHistory
                {
                    SecurityId = request.SecurityId,
                    SeriesDate = seriesDate,
                    Open = item.Open,
                    High = item.High,
                    Low = item.Low,
                    Close = item.Close,
                    Volume = item.Volumne,
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
