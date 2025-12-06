using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

namespace StockPortfolio.API.ServiceCollectionExtensions;

public static class AlphaVantageApiClientServiceExtension
{
    public static void AddAlphaVantageApiClientService(this IServiceCollection services)
    {
        services.AddScoped<IStockApiClient, AlphaVantageApiClientService>();

        services.AddTransient<InsiderTransactionsHandler>();
        services.AddTransient<NewsSentimentHandler>();
        services.AddTransient<OverviewHandler>();
        services.AddTransient<SymbolSearchHandler>();

        services.AddTransient<TimeSeriesIntradayHandler>();
        services.AddTransient<TimeSeriesDailyHandler>();
        services.AddTransient<TimeSeriesMonthlyHandler>();
        services.AddTransient<TimeSeriesWeeklyHandler>();
    }
}
