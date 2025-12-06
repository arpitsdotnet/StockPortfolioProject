using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

public sealed record OverviewRequest(string Symbol);
public sealed record OverviewResponse(string Symbol);
public class OverviewHandler
{
    private readonly IStockApiClient _stockApiClient;

    public OverviewHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }
    public async Task<Result<OverviewResponse>> Handle(OverviewRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return Result<OverviewResponse>.Failure(
                new Error(
                    ErrorType.VALIDATION,
                    ErrorCode.BAD_REQUEST,
                    "Request should not be empty."
                )
            );
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.OVERVIEW}&" +
            $"symbol={request.Symbol}";

        var response = await _stockApiClient.Query<OverviewResponse>(query, cancellationToken);

        return response;
    }
    
}
