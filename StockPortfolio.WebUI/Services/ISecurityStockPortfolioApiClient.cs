using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public interface ISecurityStockPortfolioApiClient
{
    Task<IReadOnlyList<StockSecurity>> AddAsync(string keyword);
    Task<IReadOnlyList<StockSecurity>> SearchAsync(PageSetting pageSetting, string keyword = "");
}
