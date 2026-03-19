using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace StockPortfolio.WebUI.Services.Securities;

public class SharePricesService(IStockPortfolioApiClient client)
{
    private readonly IStockPortfolioApiClient _client = client;
    private const string BASE_ENDPOINT = "SharePrices";

    public async Task<IReadOnlyList<SharePrice>> GetSharePricesAsync(PageSetting pageSetting, string keyword, string fromDateString, string toDateString, CancellationToken cancellationToken)
    {
        // Example endpoint – replace with real API
        string requestUri = $"{BASE_ENDPOINT}?keyword={keyword}&fromDateString={fromDateString}&toDateString={toDateString}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}";

        var response = await _client.GetAsync<List<SharePrice>>(requestUri, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<SharePrice> CreateSharePricesAsync(SharePrice stock, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsync(BASE_ENDPOINT, stock, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<SharePrice> UpdateSharePricesAsync(int id, SharePrice stock, CancellationToken cancellationToken)
    {
        string requestUri = $"{BASE_ENDPOINT}?Id={id}";

        var response = await _client.PutAsync(BASE_ENDPOINT, stock, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }

    public async Task<SharePrice> DeleteSharePricesAsync(int id, CancellationToken cancellationToken)
    {
        string requestUri = $"{BASE_ENDPOINT}?Id={id}";

        var response = await _client.DeleteAsync<SharePrice>(BASE_ENDPOINT, cancellationToken);

        if (response.IsSuccess == false)
            throw new ArgumentException(response.Error?.Description);

        return response.Value!;
    }
}
