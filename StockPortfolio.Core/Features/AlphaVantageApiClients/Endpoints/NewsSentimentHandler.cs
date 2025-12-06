using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

public sealed record NewsSentimentRequest(string Symbol);
public sealed record NewsSentimentResponse(string Symbol);
public class NewsSentimentHandler
{
    private readonly IStockApiClient _alphaVantageApiClient;

    public NewsSentimentHandler(IStockApiClient alphaVantageApiClient)
    {
        _alphaVantageApiClient = alphaVantageApiClient;
    }
    public async Task<Result<NewsSentimentResponse>> Handle(NewsSentimentRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.NEWS_SENTIMENT}&" +
            $"symbol={request.Symbol}";

        var response = await _alphaVantageApiClient.Query<NewsSentimentResponse>(query, cancellationToken);

        return response;
    }

}
