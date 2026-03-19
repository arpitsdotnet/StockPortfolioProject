using System.Text.Json;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public class StockPortfolioApiClientService(
    ILogger<StockPortfolioApiClientService> logger,
    HttpClient httpClientFactory) : IStockPortfolioApiClient
{
    private readonly ILogger<StockPortfolioApiClientService> _logger = logger;
    private readonly HttpClient _httpClient = httpClientFactory;

    public async Task<ResultDto<T>> GetAsync<T>(string query, CancellationToken cancellationToken) where T : class
    {
        try
        {
            _logger.LogInformation("Stock Portfolio API Client - Get Request Called: Query: {query}.", query);

            var apiResponse = await _httpClient.GetFromJsonAsync<ResultDto<T>>(query, cancellationToken);

            _logger.LogInformation("Stock Portfolio API Client - Get Request Completed");

            return apiResponse ?? throw new ArgumentException("Unable to get response from API, please try again or contact administration.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ResultDto<T>()
            {
                IsSuccess = false,
                Error = new Error(
                    ErrorType.FAILURE,
                    ErrorCode.INTERNAL_SERVER_ERROR,
                    "Oops! Something wend wrong, please contact administration.")
            };
        }
    }

    public async Task<ResultDto<T>> PostAsync<T>(string query, T request, CancellationToken cancellationToken) where T : class
    {
        try
        {
            _logger.LogInformation("Stock Portfolio API Client - Post Request Called: Query: {query}, Request: {request}.", query, JsonSerializer.Serialize(request));

            var apiResponse = await _httpClient.PostAsJsonAsync<T>(query, request, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();

            if (apiResponse == null)
                throw new Exception("Unable to get response from API, please try again or contact administration.");

            var result = await apiResponse.Content.ReadFromJsonAsync<ResultDto<T>>(cancellationToken);

            _logger.LogInformation("Stock Portfolio API Client - Post Request Completed");

            return result ?? throw new ArgumentException("API response is empty, please try again or contact administration.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ResultDto<T>()
            {
                IsSuccess = false,
                Error = new Error(
                    ErrorType.FAILURE,
                    ErrorCode.INTERNAL_SERVER_ERROR,
                    "Oops! Something wend wrong, please contact administration.")
            };
        }
    }

    public async Task<ResultDto<T>> PutAsync<T>(string query, T request, CancellationToken cancellationToken) where T : class
    {
        try
        {
            _logger.LogInformation("Stock Portfolio API Client - Put Request Called: Query: {query}, Request: {request}.", query, JsonSerializer.Serialize(request));

            var apiResponse = await _httpClient.PutAsJsonAsync<T>(query, request, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();

            if (apiResponse == null)
                throw new Exception("Unable to get response from API, please try again or contact administration.");

            var result = await apiResponse.Content.ReadFromJsonAsync<ResultDto<T>>(cancellationToken);

            _logger.LogInformation("Stock Portfolio API Client - Put Request Completed");

            return result ?? throw new ArgumentException("API response is empty, please try again or contact administration.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ResultDto<T>()
            {
                IsSuccess = false,
                Error = new Error(
                    ErrorType.FAILURE,
                    ErrorCode.INTERNAL_SERVER_ERROR,
                    "Oops! Something wend wrong, please contact administration.")
            };
        }
    }

    public async Task<ResultDto<T>> DeleteAsync<T>(string query, CancellationToken cancellationToken) where T : class
    {
        try
        {
            _logger.LogInformation("Stock Portfolio API Client - Delete Request Called: Query: {query}.", query);

            var apiResponse = await _httpClient.DeleteAsync(query, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();

            if (apiResponse == null)
                throw new Exception("Unable to get response from API, please try again or contact administration.");

            var result = await apiResponse.Content.ReadFromJsonAsync<ResultDto<T>>(cancellationToken);

            _logger.LogInformation("Stock Portfolio API Client - Delete Request Completed");

            return result ?? throw new ArgumentException("API response is empty, please try again or contact administration.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new ResultDto<T>()
            {
                IsSuccess = false,
                Error = new Error(
                    ErrorType.FAILURE,
                    ErrorCode.INTERNAL_SERVER_ERROR,
                    "Oops! Something wend wrong, please contact administration.")
            };
        }
    }
}
