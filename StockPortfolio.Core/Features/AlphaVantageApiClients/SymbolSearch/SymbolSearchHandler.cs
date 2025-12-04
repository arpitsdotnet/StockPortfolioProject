using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.SymbolSearch;

public sealed record SymbolSearchRequest(string Keywords);
public sealed record SymbolSearchResponse(string? Symbol, string? Name, string? Exchange, string? Type, string? Currency);

public class SymbolSearchHandler
{
    private readonly IStockApiClient _stockApiClient;

    public SymbolSearchHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }

    public async Task<Result<List<SymbolSearchResponse>>> Handle(SymbolSearchRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.SYMBOL_SEARCH}&" +
            $"keywords={request.Keywords}";

        var apiResponse = await _stockApiClient.Query<SymbolSearchResponse_BestMatches>(query, cancellationToken);

        if (apiResponse.IsFailure)
        {
            return apiResponse.Error;
        }

        List<SymbolSearchResponse> response = (from item in apiResponse.Value.DataList
                                           select new SymbolSearchResponse(
                                               Symbol: item.Symbol,
                                               Name: item.Name,
                                               Exchange: item.Region,
                                               Type: item.Type,
                                               Currency: item.Currency)
                                           ).ToList();

        if (response == null || response.Count <= 0)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Response is empty.");
        }

        return Result<List<SymbolSearchResponse>>.Success(response);
    }

    internal class SymbolSearchResponse_BestMatches
    {
        [JsonPropertyName("bestMatches")]
        public List<SymbolSearchResponse_BestMatches_Data>? DataList { get; set; }
    }
    internal class SymbolSearchResponse_BestMatches_Data
    {
        [JsonPropertyName("1. symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("2. name")]
        public string? Name { get; set; }

        [JsonPropertyName("3. type")]
        public string? Type { get; set; }

        [JsonPropertyName("4. region")]
        public string? Region { get; set; }

        [JsonPropertyName("5. marketOpen")]
        public string? MarketOpen { get; set; }

        [JsonPropertyName("6. marketClose")]
        public string? MarketClose { get; set; }

        [JsonPropertyName("7. timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("8. currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("9. matchScore")]
        public string? MatchScore { get; set; }
    }

}
