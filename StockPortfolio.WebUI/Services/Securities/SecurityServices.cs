using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services.Securities;

public class SecurityServices(IStockPortfolioApiClient client)
{
    private readonly IStockPortfolioApiClient _client = client;
    private const string BASE_ENDPOINT = "MockStocks";

    public async Task<StockSecurity> CreateSecurityAsync(StockSecurity stock, CancellationToken cancellationToken)
    {
        var response = await _client.PushAsync<StockSecurity>(BASE_ENDPOINT, stock, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<IReadOnlyList<StockSecurity>> SearchSecurityAsync(PageSetting pageSetting, string keyword, CancellationToken cancellationToken)
    {
        // Example endpoint – replace with real API
         string requestUri = $"{BASE_ENDPOINT}?keyword={keyword}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}";

        var response = await _client.GetAsync<List<StockSecurity>>(requestUri, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }
}
