
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.Core.Contracts;
public interface IStockApiClient
{
    Task<Result<TResponse>> Query<TResponse>(string query, CancellationToken cancellationToken) where TResponse : class;
}
