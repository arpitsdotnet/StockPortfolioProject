using System.Net.Http.Json;
using System.Text.Json;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients;

public class AlphaVantageApiClientHandler : IStockApiClient
{
    private const string _BaseUrl = "https://www.alphavantage.co/";
    private readonly HttpClient _httpClient;

    public AlphaVantageApiClientHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<TResponse>> Query<TResponse>(string query, CancellationToken cancellationToken) where TResponse : class
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "API Query should not be empty.");
        }

        _httpClient.BaseAddress = new Uri(_BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        try
        {
            var apiResponse = await _httpClient.GetFromJsonAsync<TResponse>(query, cancellationToken);
            //var apiStringResponse = await _httpClient.GetAsync(query, cancellationToken);
            //apiStringResponse.EnsureSuccessStatusCode();

            //string json = await apiStringResponse.Content.ReadAsStringAsync(cancellationToken);
            //var apiResponse = JsonSerializer.Deserialize<TResponse>(json);

            if (apiResponse == null)
            {
                return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "API Response is empty.");
            }

            return Result<TResponse>.Success(apiResponse);
        }
        catch (Exception ex)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.INTERNAL_SERVER_ERROR, ex.Message);
        }
    }


}
