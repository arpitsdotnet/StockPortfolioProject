using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using System.Text.Json;

// Local fake that returns JSON for requested response type and deserializes to TResponse
public class LocalFakeStockApiClient : IStockApiClient
{
    private readonly System.Func<System.Type, string?> _jsonFactory;
    public LocalFakeStockApiClient(System.Func<System.Type, string?> jsonFactory) => _jsonFactory = jsonFactory;

    public Task<StockPortfolio.Core.BaseModels.Result<TResponse>> Query<TResponse>(string query, System.Threading.CancellationToken cancellationToken) where TResponse : class
    {
        var t = typeof(TResponse);
        var json = _jsonFactory?.Invoke(t);
        if (string.IsNullOrEmpty(json))
        {
            return Task.FromResult(StockPortfolio.Core.BaseModels.Result<TResponse>.Failure(new StockPortfolio.Core.BaseModels.Error(StockPortfolio.Core.BaseModels.ErrorType.FAILURE, StockPortfolio.Core.BaseModels.ErrorCode.NOT_FOUND, "not found")));
        }

        try
        {
            var obj = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (obj == null)
                return Task.FromResult(StockPortfolio.Core.BaseModels.Result<TResponse>.Failure(new StockPortfolio.Core.BaseModels.Error(StockPortfolio.Core.BaseModels.ErrorType.FAILURE, StockPortfolio.Core.BaseModels.ErrorCode.NOT_FOUND, "deserialized null")));

            return Task.FromResult(StockPortfolio.Core.BaseModels.Result<TResponse>.Success(obj));
        }
        catch (System.Exception ex)
        {
            return Task.FromResult(StockPortfolio.Core.BaseModels.Result<TResponse>.Failure(new StockPortfolio.Core.BaseModels.Error(StockPortfolio.Core.BaseModels.ErrorType.FAILURE, StockPortfolio.Core.BaseModels.ErrorCode.INTERNAL_SERVER_ERROR, ex.Message)));
        }
    }
}

public class AlphaVantageControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AlphaVantageControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SymbolSearch_ReturnsResults()
    {
        // Replace IStockApiClient with fake that returns a predictable JSON response
        using var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IStockApiClient>(_ => new LocalFakeStockApiClient((t) =>
                {
                    var fullname = t.FullName ?? string.Empty;

                    // The controller currently uses TimeSeriesDailyHandler for the "search" endpoint.
                    // Return a TimeSeriesDailyResponse_Body shaped JSON so the handler will parse it successfully.
                    if (fullname.Contains("TimeSeriesDailyResponse_Body"))
                    {
                        // Provide JSON matching the TimeSeriesDailyResponse_Body shape
                        return "{\n  \"Meta Data\": { \"1. Information\": \"Daily Prices\", \"5. Time Zone\": \"UTC\" },\n  \"Time Series (Daily)\": {\n    \"2026-03-10\": {\n      \"1. open\": 1.0,\n      \"2. high\": 2.0,\n      \"3. low\": 0.5,\n      \"4. close\": 1.5,\n      \"5. volumn\": 100\n    }\n  }\n}";
                    }

                    // Also support SymbolSearch response type if needed
                    if (fullname.Contains("SymbolSearchResponse_BestMatches"))
                    {
                        return "{\n  \"bestMatches\": [\n    {\n      \"1. symbol\": \"TST\",\n      \"2. name\": \"Test Co\",\n      \"3. type\": \"Equity\",\n      \"4. region\": \"NASDAQ\",\n      \"8. currency\": \"USD\"\n    }\n  ]\n}";
                    }

                    // default: return empty
                    return null;
                }));
            });
        }).CreateClient();

        var resp = await client.GetAsync("/api/securityapi/search?keywords=test");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.TryGetProperty("isSuccess", out var isSuccess));
        Assert.True(isSuccess.GetBoolean());
    }
}
