using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolio.WebUI.Models;
using StockPortfolio.WebUI.Models.BaseModels;
using StockPortfolio.WebUI.Services;
using System.Net.Http.Json;

namespace StockPortfolio.WebUI.Pages.Stocks;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ISecurityStockPortfolioApiClient _stockPortfolioApiService;
    private readonly HttpClient _httpClient;

    public IndexModel(
        ILogger<IndexModel> logger,
        ISecurityStockPortfolioApiClient stockPortfolioApiService,
        IHttpClientFactory httpClientFactory)
    {
        _stockPortfolioApiService = stockPortfolioApiService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public string? ErrorMessage { get; private set; }

    public IReadOnlyList<StockSecurity> Securities { get; private set; }
        = Array.Empty<StockSecurity>();

    [BindProperty(SupportsGet = true)]
    public string? SearchKeywords { get; set; }

    public List<StockSecurity> SearchResults { get; set; } = new();

    [BindProperty]
    public string? SelectedSymbol { get; set; }
    [BindProperty]
    public string? SelectedName { get; set; }
    [BindProperty]
    public string? SelectedExchange { get; set; }
    [BindProperty]
    public string? SelectedType { get; set; }
    [BindProperty]
    public string? SelectedCurrency { get; set; }

    public bool ShowModal { get; set; } = false;

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Fetching stock securities started.");
        try
        {
            PageSetting pageSetting = new();

            Securities = await _stockPortfolioApiService.SearchAsync(pageSetting, "", default);

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

    // Open the Add Security modal (server side)
    public IActionResult OnGetOpenAdd()
    {
        ShowModal = true;
        return Page();
    }

    // Server-side search handler (form POST)
    public async Task<IActionResult> OnPostSearchAsync(CancellationToken cancellationToken)
    {
        ShowModal = true;

        try
        {
            PageSetting pageSetting = new();
            var list = await _stockPortfolioApiService.SearchAsync(pageSetting, SearchKeywords ?? string.Empty, cancellationToken);

            SearchResults = list.Select(s => StockSecurity.Create(s.Symbol, s.Name, s.Exchange, s.SecurityType, s.Currency)).ToList();

            if (SearchResults.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No results found.");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while searching securities from UI");
            ModelState.AddModelError(string.Empty, "Search failed.");
            return Page();
        }
    }

    // Server-side create handler (form POST)
    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        ShowModal = true; // in case we need to re-open on error

        if (string.IsNullOrWhiteSpace(SelectedSymbol) || string.IsNullOrWhiteSpace(SelectedName))
        {
            ModelState.AddModelError(string.Empty, "Symbol and Name are required.");
            return Page();
        }

        var payload = new CreateSecurityRequest(SelectedSymbol, SelectedName, SelectedExchange, SelectedType, SelectedCurrency, null, null, null);

        try
        {
            var resp = await _httpClient.PostAsJsonAsync("/api/Security", payload, cancellationToken);

            if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var body = await resp.Content.ReadFromJsonAsync<ResultDto<object>>(cancellationToken: cancellationToken);
                ModelState.AddModelError(string.Empty, body?.Error?.Description ?? "A security with this symbol already exists.");
                return Page();
            }

            if (!resp.IsSuccessStatusCode)
            {
                var text = await resp.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Create security failed: {Text}", text);
                ModelState.AddModelError(string.Empty, "Create failed.");
                return Page();
            }

            // success - redirect to refresh list and avoid repost
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating security from UI");
            ModelState.AddModelError(string.Empty, "Create failed.");
            return Page();
        }
    }
}

public record CreateSecurityRequest(string Symbol, string Name, string? Exchange, string? SecurityType, string? Currency, string? ISIN, string? Sector, string? Industry);
