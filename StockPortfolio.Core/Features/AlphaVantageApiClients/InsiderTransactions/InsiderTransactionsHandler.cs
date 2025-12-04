using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.InsiderTransactions;

public sealed record InsiderTransactionsRequest(string Symbol);
public sealed record InsiderTransactionsResponse(string Symbol);
public class InsiderTransactionsHandler
{
    private readonly IStockApiClient _alphaVantageApiClient;

    public InsiderTransactionsHandler(IStockApiClient alphaVantageApiClient)
    {
        _alphaVantageApiClient = alphaVantageApiClient;
    }
    public async Task<Result<InsiderTransactionsResponse>> Handle(InsiderTransactionsRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return Result<InsiderTransactionsResponse>.Failure(
                new Error(
                    ErrorType.VALIDATION,
                    ErrorCode.BAD_REQUEST,
                    "Request should not be empty."
                )
            );
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.INSIDER_TRANSACTIONS}&" +
            $"symbol={request.Symbol}";

        var response = await _alphaVantageApiClient.Query<InsiderTransactionsResponse>(query, cancellationToken);

        return response;
    }
    
}
