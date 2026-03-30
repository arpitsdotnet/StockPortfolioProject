using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;
using StockPortfolio.Core.Features.Securities.CreateSecurity;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Features.SharePrices;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.SharePrices.UpdateSharePrice;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.UnitTests.TestHelpers;
using Xunit;

namespace StockPortfolio.Core.UnitTests.SharePrices;

public class FetchAndStoreSharePricesHandlerTests
{
    [Fact]
    public async Task Handle_FetchesAndStoresPrices()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_FetchPrices")
            .Options;

        await using var fakeContext = new ApplicationDbContext(options);
        // seed security
        var sec = new Security { Symbol = "TST", Name = "Test Co", IsActive = true, CreatedOn = DateTime.UtcNow, CreatedById = 0, LastModifiedOn = DateTime.UtcNow, LastModifiedById = 0 };
        await fakeContext.Securities.AddAsync(sec);
        await fakeContext.SaveChangesAsync();

        // prepare fake daily body
        var body = new TimeSeriesDailyStockApiHandler.TimeSeriesDailyResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { TimeZone = "UTC" },
            TimeSeriesDaily = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.Date.ToString("yyyy-MM-dd")] = new TimeSeries_ItemResponse { Open = 10, High = 12, Low = 9, Close = 11, Volume = 1000 }
            }
        };

        var fakeClient = new FakeStockApiClient((t) => t == typeof(TimeSeriesDailyStockApiHandler.TimeSeriesDailyResponse_Body) ? (object)body : null);
        var fakeDailyHandler = new TimeSeriesDailyStockApiHandler(fakeClient);

        var fakeCreateRequest = new CreateSecurityHandler(fakeContext);

        var handler = new FetchAndStoreSharePricesHandler(fakeContext, fakeDailyHandler, fakeCreateRequest);

        var request = new FetchAndStoreSharePricesRequest(sec.Symbol, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1),
            sec.Name, sec.Exchange, sec.SecurityType, sec.Currency, sec.ISIN, sec.Sector, sec.Industry);

        Result<List<SharePriceHistory>> result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);

        var stored = await fakeContext.SharePriceHistories.FirstOrDefaultAsync();
        Assert.NotNull(stored);
        Assert.Equal(11, stored.Close);
    }
}
