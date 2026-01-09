using StockPortfolio.WebUI.Models;

namespace StockPortfolio.WebUI.Services;

public interface IStockPortfolioApiClient
{
    Task<IReadOnlyList<StockSecurity>> GetSecuritiesAsync(int page = 0, int pageSize = 10, string keyword = "");
}
