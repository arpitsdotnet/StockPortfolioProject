using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;

namespace StockPortfolio.API.ServiceCollectionExtensions;

public static class AlphaVantageApiClientServiceExtension
{
    public static void AddAlphaVantageApiClientService(this IServiceCollection services)
    {
        services.AddScoped<IStockApiClient, AlphaVantageApiClientService>();

        services.AddTransient<InsiderTransactionsStockApiHandler>();
        services.AddTransient<NewsSentimentStockApiHandler>();
        services.AddTransient<OverviewStockApiHandler>();
        services.AddTransient<SymbolSearchStockApiHandler>();

        services.AddTransient<TimeSeriesIntradayStockApiHandler>();
        services.AddTransient<TimeSeriesDailyStockApiHandler>();
        services.AddTransient<TimeSeriesMonthlyStockApiHandler>();
        services.AddTransient<TimeSeriesWeeklyStockApiHandler>();

        // Securities handlers
        services.AddTransient<DeleteSecurityHandler>();
    }
}
