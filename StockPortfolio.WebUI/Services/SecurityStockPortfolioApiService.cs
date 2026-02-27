using System.Text.Json;
using Microsoft.Extensions.Logging;
using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;

namespace StockPortfolio.WebUI.Services;

public class SecurityStockPortfolioApiService(
    ILogger<SecurityStockPortfolioApiService> logger,
    HttpClient httpClient) : ISecurityStockPortfolioApiClient
{
    private readonly ILogger<SecurityStockPortfolioApiService> logger = logger;
    private readonly HttpClient httpClient = httpClient;

    public async Task<StockSecurity> AddAsync(StockSecurity stock, CancellationToken cancellationToken)
    {
        var requestUId = Guid.NewGuid().ToString();

        logger.LogInformation("Stock Add API called with data: {stock}.", JsonSerializer.Serialize<StockSecurity>(stock));
        // Example endpoint – replace with real API
        var apiResponse = await httpClient.PostAsJsonAsync<StockSecurity>("stocks", stock, cancellationToken);
        apiResponse.EnsureSuccessStatusCode();

        if (apiResponse == null || apiResponse.IsSuccessStatusCode == false)
            throw new Exception("Unable to receive API response.");

        ResultDto<StockSecurity> result = await apiResponse.Content.ReadFromJsonAsync<ResultDto<StockSecurity>>(cancellationToken)
            ?? throw new Exception("Unable to read result from the API.");

        if (result.IsSuccess == false)
            throw new ArgumentException(result.Error?.Description);

        return result.Value!;
    }

    public async Task<IReadOnlyList<StockSecurity>> SearchAsync(PageSetting pageSetting, string keyword, CancellationToken cancellationToken)
    {
        // Example endpoint – replace with real API
        string requestUri = $"stocks?keyword={keyword}&page={pageSetting.Page}&pageSize={pageSetting.PageSize}";

        var apiResponse = await httpClient.GetFromJsonAsync<ResultDto<List<StockSecurity>>>(requestUri, cancellationToken)
            ?? throw new Exception("Unable to receive API response.");

        if (apiResponse.IsSuccess == false)
            throw new ArgumentException(apiResponse.Error?.Description);

        return apiResponse.Value!;
    }
}
