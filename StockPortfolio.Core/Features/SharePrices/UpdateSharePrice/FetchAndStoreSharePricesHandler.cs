using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Features.Securities.CreateSecurity;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.Features.SharePrices.UpdateSharePrice;

public sealed record FetchAndStoreSharePricesRequest(string Symbol, DateTime FromDate, DateTime ToDate,
    string? Name, string? Exchange, string? SecurityType, string? Currency, string? ISIN, string? Sector, string? Industry);

public class FetchAndStoreSharePricesHandler
{
    private readonly ApplicationDbContext _context;
    private readonly CreateSecurityHandler _createSecurityHandler;
    private readonly TimeSeriesDailyStockApiHandler _timeSeriesDailyHandler;

    public FetchAndStoreSharePricesHandler(
        ApplicationDbContext context,
        TimeSeriesDailyStockApiHandler timeSeriesDailyHandler,
        CreateSecurityHandler createSecurityHandler)
    {
        _context = context;
        _timeSeriesDailyHandler = timeSeriesDailyHandler;
        _createSecurityHandler = createSecurityHandler;
    }

    public async Task<Result<List<SharePriceHistory>>> Handle(FetchAndStoreSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrEmpty(request.Symbol))
            return Result<List<SharePriceHistory>>
                .Failure(new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid request"));

        int securityId = 0;

        var security = await _context.Securities.FirstOrDefaultAsync(s => s.Symbol == request.Symbol, cancellationToken);
        if (security == null)
        {
            var createSecurityResponse = await _createSecurityHandler.Handle(new CreateSecurityRequest(
                request.Symbol, request.Name!, request.Exchange, request.SecurityType,
                request.Currency, request.ISIN, request.Sector, request.Industry), cancellationToken);

            if (createSecurityResponse.IsFailure)
                return Result<List<SharePriceHistory>>
                    .Failure(createSecurityResponse.Error);

            securityId = createSecurityResponse.Value.SecurityId;
        }

        // Fetch daily series via handler
        var apiResult = await _timeSeriesDailyHandler.Handle(new TimeSeriesDailyRequest(request.Symbol!), cancellationToken);
        if (apiResult.IsFailure)
            return Result<List<SharePriceHistory>>
                .Failure(apiResult.Error);

        var items = apiResult.Value;

        // Filter by date range
        var filtered = items
            .Select(i =>
            {
                if (!DateTime.TryParse(i.SeriesDateTime, out var dt)) return (Parsed: false, Date: default, Item: i);
                return (Parsed: true, dt.Date, Item: i);
            })
            .Where(p => p.Parsed && p.Date >= request.FromDate.Date && p.Date <= request.ToDate.Date)
            .ToList();

        if (!filtered.Any())
            return Result<List<SharePriceHistory>>
                .Failure(new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "No price data found for date range."));

        var dates = filtered.Select(f => f.Date).Distinct().ToList();

        var existingHistories = await _context.SharePriceHistories
            .Where(x => x.SecurityId == securityId && dates.Contains(x.SeriesDate))
            .ToListAsync(cancellationToken);
        
        var existingByDate = existingHistories.ToDictionary(h => h.SeriesDate, h => h);

        // Map to domain and upsert (avoid duplicates)
        int inserted = 0, updated = 0;
        foreach (var f in filtered)
        {
            var apiItem = f.Item;
            var seriesDate = f.Date;

            if (existingByDate.TryGetValue(seriesDate, out var exists))
            {
                updated++;
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
                inserted++;
                var entity = new SharePriceHistory
                {
                    SecurityId = securityId,
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
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<List<SharePriceHistory>>.Success(existingHistories, $"Inserted {inserted} / Updated {updated} records.");
    }
}
