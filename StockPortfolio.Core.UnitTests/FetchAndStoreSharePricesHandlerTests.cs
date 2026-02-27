using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.SharePrices;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.BaseModels;
using System.Threading;
using StockPortfolio.Core.UnitTests.TestHelpers;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;

namespace StockPortfolio.Core.UnitTests;

public class FetchAndStoreSharePricesHandlerTests
{
    [Fact]
    public async Task Handle_FetchesAndStoresPrices()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_FetchPrices")
            .Options;

        await using var context = new ApplicationDbContext(options);
        // seed security
        var sec = new Security { Symbol = "TST", Name = "Test Co", IsActive = true, CreatedOn = DateTime.UtcNow, CreatedById = 0, LastModifiedOn = DateTime.UtcNow, LastModifiedById = 0 };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        // prepare fake daily body
        var body = new TimeSeriesDailyHandler.TimeSeriesDailyResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { TimeZone = "UTC" },
            TimeSeriesDaily = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.Date.ToString("yyyy-MM-dd")] = new TimeSeries_ItemResponse { Open = 10, High = 12, Low = 9, Close = 11, Volume = 1000 }
            }
        };

        var fakeClient = new FakeStockApiClient((Type t) => t == typeof(TimeSeriesDailyHandler.TimeSeriesDailyResponse_Body) ? (object?)body : null);
        var realDailyHandler = new TimeSeriesDailyHandler(fakeClient);

        var handler = new FetchAndStoreSharePricesHandler(context, realDailyHandler);

        var request = new FetchAndStoreSharePricesRequest(sec.SecurityId, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);

        var stored = await context.SharePriceHistories.FirstOrDefaultAsync();
        Assert.NotNull(stored);
        Assert.Equal(11, stored.Close);
    }
}
