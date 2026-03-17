using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public interface IStockPortfolioApiClient
{
    Task<ResultDto<T>> GetAsync<T>(string query, CancellationToken cancellationToken);
    Task<ResultDto<T>> PushAsync<T>(string query, T request, CancellationToken cancellationToken);
}
