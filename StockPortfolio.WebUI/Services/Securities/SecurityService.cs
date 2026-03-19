using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace StockPortfolio.WebUI.Services.Securities;

public class SecurityService(IStockPortfolioApiClient client)
{
    private readonly IStockPortfolioApiClient _client = client;
    private const string BASE_ENDPOINT = "MockStocks";

    public async Task<IReadOnlyList<StockSecurity>> SearchSecurityAsync(PageSetting pageSetting, string keyword, CancellationToken cancellationToken)
    {
        // Example endpoint – replace with real API
        string requestUri = $"{BASE_ENDPOINT}?keyword={keyword}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}";

        var response = await _client.GetAsync<List<StockSecurity>>(requestUri, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<StockSecurity> CreateSecurityAsync(StockSecurity stock, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsync(BASE_ENDPOINT, stock, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<StockSecurity> UpdateSecurityAsync(int id, StockSecurity stock, CancellationToken cancellationToken)
    {
        string requestUri = $"{BASE_ENDPOINT}?Id={id}";

        var response = await _client.PutAsync(BASE_ENDPOINT, stock, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<StockSecurity> DeleteSecurityAsync(int id, CancellationToken cancellationToken)
    {
        string requestUri = $"{BASE_ENDPOINT}?Id={id}";

        var response = await _client.DeleteAsync<StockSecurity>(BASE_ENDPOINT, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }
}
