using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public interface IStockPortfolioApiClient
{
    Task<ResultDto<T>> GetAsync<T>(string query, CancellationToken cancellationToken) where T : class;
    Task<ResultDto<T>> PostAsync<T>(string query, T request, CancellationToken cancellationToken) where T : class;
    Task<ResultDto<T>> PutAsync<T>(string query, T request, CancellationToken cancellationToken) where T : class;
    Task<ResultDto<T>> DeleteAsync<T>(string query, CancellationToken cancellationToken) where T : class;
}
