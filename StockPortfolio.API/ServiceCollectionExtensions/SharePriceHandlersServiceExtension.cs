using StockPortfolio.Core.Features.SharePrices.GetSharePrices;
using StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;
using StockPortfolio.Core.Features.SharePrices.CreateSharePrice;
using StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;

namespace StockPortfolio.API.ServiceCollectionExtensions;

public static class SharePriceHandlersServiceExtension
{
    public static void AddSharePriceHandlers(this IServiceCollection services)
    {
        services.AddTransient<CreateSharePriceHandler>();
        services.AddTransient<DeleteSharePriceHandler>();
        services.AddTransient<GetSharePricesHandler>();
        services.AddTransient<GetLatestSharePriceHandler>();
    }
}
