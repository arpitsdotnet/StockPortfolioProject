using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public class SecurityStockPortfolioApiService(HttpClient httpClient) : ISecurityStockPortfolioApiClient
{
    public Task<IReadOnlyList<StockSecurity>> AddAsync(string keyword)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<StockSecurity>> SearchAsync(PageSetting pageSetting, string keyword)
    {
        // Example endpoint – replace with real API
        var response = await httpClient.GetFromJsonAsync<Result<List<StockSecurity>>>(
            $"stocks?keyword={keyword}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}"
        );

        if (response == null)
            throw new ArgumentException("Oops! Something went wrong, we did not receive any response, please contact administration.");

        if (response.IsFailure)
            throw new ArgumentException(response.Error.Description);

        return response.Value;
    }
}
