using Microsoft.Extensions.DependencyInjection;
using StockPortfolio.Core.Features.Securities.CreateSecurity;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;
using StockPortfolio.Core.Features.Securities.UpdateSecurity;
using StockPortfolio.Core.Features.SharePrices;

namespace StockPortfolio.API.ServiceCollectionExtensions;

public static class SecurityHandlersServiceExtension
{
    public static void AddSecurityHandlers(this IServiceCollection services)
    {
        services.AddTransient<CreateSecurityHandler>();
        services.AddTransient<DeleteSecurityHandler>();
        services.AddTransient<UpdateSecurityHandler>();
        services.AddTransient<FetchAndStoreSharePricesHandler>();
    }
}
