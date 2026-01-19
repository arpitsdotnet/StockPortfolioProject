using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public class SecurityStockPortfolioApiService : ISecurityStockPortfolioApiClient
{
    private readonly HttpClient _httpClient;

    public SecurityStockPortfolioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<StockSecurity>> SearchAsync(PageSetting pageSetting, string keyword)
    {
        // Example endpoint – replace with real API
        var response = await _httpClient.GetFromJsonAsync<Result<List<StockSecurity>>>(
            $"stocks?keyword={keyword}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}"
        );

        if (response == null)
            throw new ArgumentException("Oops! Something went wrong, we did not receive any response, please contact administration.");

        if (response.IsFailure)
            throw new ArgumentException(response.Error.Description);

        return response.Value;
    }
}
