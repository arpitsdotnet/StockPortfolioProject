using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public class StockPortfolioApiService : IStockPortfolioApiClient
{
    private readonly HttpClient _httpClient;

    public StockPortfolioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<StockSecurity>> GetSecuritiesAsync(int page, int pageSize, string keyword)
    {
        // Example endpoint – replace with real API
        var response = await _httpClient.GetFromJsonAsync<Result<List<StockSecurity>>>(
            $"stocks?keyword={keyword}&page={page}&pageSize={pageSize}"
        );

        if (response == null)
            throw new ArgumentException("Oops! Something went wrong, we did not receive any response, please contact administration.");

        if (response.IsFailure)
            throw new ArgumentException(response.Error.Description);

        return response.Value;
    }
}
