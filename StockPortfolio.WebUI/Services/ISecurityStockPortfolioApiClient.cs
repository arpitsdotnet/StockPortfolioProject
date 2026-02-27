using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public interface ISecurityStockPortfolioApiClient
{
    Task<StockSecurity> AddAsync(StockSecurity stock, CancellationToken cancellationToken);
    Task<IReadOnlyList<StockSecurity>> SearchAsync(PageSetting pageSetting, string keyword, CancellationToken cancellationToken);
}
