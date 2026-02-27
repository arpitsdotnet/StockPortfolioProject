using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.UnitTests.TestHelpers;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;
using StockPortfolio.Core.BaseModels;
using System.Threading;

namespace StockPortfolio.Core.UnitTests;

public class AlphaVantageHandlersTests
{
    [Fact]
    public async Task SymbolSearchHandler_ReturnsParsedList()
    {
        var bestMatches = new SymbolSearchHandler.SymbolSearchResponse_BestMatches
        {
            DataList = new List<SymbolSearchHandler.SymbolSearchResponse_BestMatches_Data>
            {
                new SymbolSearchHandler.SymbolSearchResponse_BestMatches_Data { Symbol = "TST", Name = "Test Co", Region = "NASDAQ", Type = "Equity", Currency = "USD" }
            }
        };

        var client = new FakeStockApiClient((Type t) => t == typeof(SymbolSearchHandler.SymbolSearchResponse_BestMatches) ? (object?)bestMatches : null);
        var handler = new SymbolSearchHandler(client);

        var res = await handler.Handle(new SymbolSearchRequest("test"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Single(res.Value);
        Assert.Equal("TST", res.Value[0].Symbol);
    }

    [Fact]
    public async Task TimeSeriesDailyHandler_ParsesDaily()
    {
        var body = new TimeSeriesDailyHandler.TimeSeriesDailyResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { TimeZone = "UTC" },
            TimeSeriesDaily = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.Date.ToString("yyyy-MM-dd")] = new TimeSeries_ItemResponse { Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100 }
            }
        };

        var client = new FakeStockApiClient((Type t) => t == typeof(TimeSeriesDailyHandler.TimeSeriesDailyResponse_Body) ? (object?)body : null);
        var handler = new TimeSeriesDailyHandler(client);

        var res = await handler.Handle(new TimeSeriesDailyRequest("TST"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task TimeSeriesIntradayHandler_ParsesIntraday()
    {
        var body = new TimeSeriesIntradayHandler.TimeSeriesIntradayResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { Interval = "1min" },
            TimeSeries1min = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")] = new TimeSeries_ItemResponse { Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100 }
            }
        };

        var client = new FakeStockApiClient((Type t) => t == typeof(TimeSeriesIntradayHandler.TimeSeriesIntradayResponse_Body) ? (object?)body : null);
        var handler = new TimeSeriesIntradayHandler(client);

        var res = await handler.Handle(new TimeSeriesIntradayRequest("TST", "1min"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task TimeSeriesWeeklyHandler_ParsesWeekly()
    {
        var body = new TimeSeriesWeeklyHandler.TimeSeriesWeeklyResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { TimeZone = "UTC" },
            TimeSeriesWeekly = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.Date.ToString("yyyy-MM-dd")] = new TimeSeries_ItemResponse { Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100 }
            }
        };

        var client = new FakeStockApiClient((Type t) => t == typeof(TimeSeriesWeeklyHandler.TimeSeriesWeeklyResponse_Body) ? (object?)body : null);
        var handler = new TimeSeriesWeeklyHandler(client);

        var res = await handler.Handle(new TimeSeriesWeeklyRequest("TST", ""), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task TimeSeriesMonthlyHandler_ParsesMonthly()
    {
        var body = new TimeSeriesMonthlyHandler.TimeSeriesMonthlyResponse_Body
        {
            MetaData = new TimeSeries_MetaDataResponse { TimeZone = "UTC" },
            TimeSeriesMonthly = new Dictionary<string, TimeSeries_ItemResponse>
            {
                [DateTime.UtcNow.Date.ToString("yyyy-MM-dd")] = new TimeSeries_ItemResponse { Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100 }
            }
        };

        var client = new FakeStockApiClient((Type t) => t == typeof(TimeSeriesMonthlyHandler.TimeSeriesMonthlyResponse_Body) ? (object?)body : null);
        var handler = new TimeSeriesMonthlyHandler(client);

        var res = await handler.Handle(new TimeSeriesMonthlyRequest("TST",""), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task OverviewHandler_ForwardsClientResult()
    {
        var overview = new OverviewResponse("TST");
        var client = new FakeStockApiClient((Type t) => t == typeof(OverviewResponse) ? (object?)overview : null);
        var handler = new OverviewHandler(client);

        var res = await handler.Handle(new OverviewRequest("TST"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal("TST", res.Value.Symbol);
    }

    [Fact]
    public async Task NewsSentimentHandler_ForwardsClientResult()
    {
        var ns = new NewsSentimentResponse("TST");
        var client = new FakeStockApiClient((Type t) => t == typeof(NewsSentimentResponse) ? (object?)ns : null);
        var handler = new NewsSentimentHandler(client);

        var res = await handler.Handle(new NewsSentimentRequest("TST"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal("TST", res.Value.Symbol);
    }

    [Fact]
    public async Task InsiderTransactionsHandler_ForwardsClientResult()
    {
        var it = new InsiderTransactionsResponse("TST");
        var client = new FakeStockApiClient((Type t) => t == typeof(InsiderTransactionsResponse) ? (object?)it : null);
        var handler = new InsiderTransactionsHandler(client);

        var res = await handler.Handle(new InsiderTransactionsRequest("TST"), CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal("TST", res.Value.Symbol);
    }
}
