using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using StockPortfolio.Core.Features.SharePrices.CreateSharePrice;
using StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;
using StockPortfolio.Core.Features.SharePrices.GetSharePrices;
using StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using StockPortfolio.Core.Services.DbContexts;
using Microsoft.EntityFrameworkCore;

public class SharePricesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SharePricesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Get_GetLatest_Delete_flow()
    {
        using var client = _factory.CreateClient();

        // Seed a security into the in-memory db
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
            db.Securities.Add(sec);
            await db.SaveChangesAsync();
        }

        // Create share price
        var createReq = new CreateSharePriceRequest(1, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100);
        var createResp = await client.PostAsJsonAsync("/api/shareprices", createReq);
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

        // Get prices for security
        var getResp = await client.GetAsync("/api/shareprices/1");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // Get latest
        var latestResp = await client.GetAsync("/api/shareprices/1/latest");
        Assert.Equal(HttpStatusCode.OK, latestResp.StatusCode);

        // Delete
        int spId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sp = await db.SharePriceHistories.FirstOrDefaultAsync();
            spId = sp.SharePriceHistoryId;
        }

        var delResp = await client.DeleteAsync($"/api/shareprices/{spId}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenInvalid()
    {
        using var client = _factory.CreateClient();
        var createReq = new CreateSharePriceRequest(0, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100);
        var createResp = await client.PostAsJsonAsync("/api/shareprices", createReq);
        Assert.Equal(HttpStatusCode.BadRequest, createResp.StatusCode);
    }
}
