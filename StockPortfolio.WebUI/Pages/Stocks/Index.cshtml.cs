using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;
using StockPortfolio.WebUI.Services;

namespace StockPortfolio.WebUI.Pages.Stocks;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ISecurityStockPortfolioApiClient _stockPortfolioApiService;

    public IndexModel(
        ILogger<IndexModel> logger,
        ISecurityStockPortfolioApiClient stockPortfolioApiService)
    {
        _stockPortfolioApiService = stockPortfolioApiService;
        _logger = logger;
    }

    public string? ErrorMessage { get; private set; }

    public IReadOnlyList<StockSecurity> Securities { get; private set; }
        = Array.Empty<StockSecurity>();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Fetching stock securities started.");
        try
        {
            PageSetting pageSetting = new();

            Securities = await _stockPortfolioApiService.SearchAsync(pageSetting);

            _logger.LogInformation("Successfully fetched {Count} stock securities.",
                Securities.Count);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching stock securities.");

            ErrorMessage = "Unable to retrieve stock data at the moment. Please try again later.";
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Stock API request timed out.");

            ErrorMessage = "The request to fetch stock data timed out. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error occurred while fetching stock securities.");

            ErrorMessage = "An unexpected error occurred. Our team has been notified.";
        }
    }
}
