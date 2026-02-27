using System;
using System.Threading;
using System.Threading.Tasks;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.Core.UnitTests.TestHelpers;

public class FakeStockApiClient : IStockApiClient
{
    private readonly Func<Type, object?> _factory;

    public FakeStockApiClient(Func<Type, object?> factory)
    {
        _factory = factory;
    }

    public Task<Result<TResponse>> Query<TResponse>(string query, CancellationToken cancellationToken) where TResponse : class
    {
        var t = typeof(TResponse);
        var obj = _factory?.Invoke(t) as TResponse;
        if (obj == null)
        {
            return Task.FromResult(Result<TResponse>.Failure(new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "not found")));
        }

        return Task.FromResult(Result<TResponse>.Success(obj));
    }
}
