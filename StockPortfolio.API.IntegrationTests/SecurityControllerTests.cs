using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class SecurityControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SecurityControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateSecurity_ReturnsCreated_And_GetByIdReturnsEntity()
    {
        using var client = _factory.CreateClient();

        var createObj = new
        {
            Symbol = "INT1",
            Name = "Integration Security",
            Exchange = "NASDAQ",
            SecurityType = "Equity",
            Currency = "USD",
            ISIN = (string?)null,
            Sector = "Tech",
            Industry = "Software"
        };

        var resp = await client.PostAsJsonAsync("/api/securityapi", createObj);
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

        var location = resp.Headers.Location;
        Assert.NotNull(location);

        // Extract id from location
        var segments = location!.AbsolutePath.Split('/');
        var idStr = segments[^1];
        var id = int.Parse(idStr);

        var getResp = await client.GetAsync($"/api/securityapi/{id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        var doc = await getResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(doc.TryGetProperty("isSuccess", out var isSuccess));
        Assert.True(isSuccess.GetBoolean());
    }

    [Fact]
    public async Task UpdateSecurity_UpdatesEntity()
    {
        using var client = _factory.CreateClient();

        int createdId;
        // Seed security
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sec = new Security { Symbol = "UPD1", Name = "To Update", IsActive = true, CreatedOn = System.DateTime.UtcNow };
            db.Securities.Add(sec);
            await db.SaveChangesAsync();
            createdId = sec.SecurityId;
        }

        var updateObj = new
        {
            SecurityId = createdId,
            Symbol = "UPD1",
            Name = "Updated Name",
            Exchange = "NYSE",
            SecurityType = "Equity",
            Currency = "USD",
            ISIN = (string?)null,
            Sector = (string?)null,
            Industry = (string?)null
        };

        var putResp = await client.PutAsJsonAsync($"/api/securityapi/{createdId}", updateObj);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        // Verify in DB
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sec = await db.Securities.FindAsync(new object[] { createdId }, default);
            Assert.NotNull(sec);
            Assert.Equal("Updated Name", sec.Name);
        }
    }

    [Fact]
    public async Task DeleteSecurity_MarksInactive()
    {
        using var client = _factory.CreateClient();

        int createdId;
        // Seed security
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sec = new Security { Symbol = "DEL1", Name = "To Delete", IsActive = true, CreatedOn = System.DateTime.UtcNow };
            db.Securities.Add(sec);
            await db.SaveChangesAsync();
            createdId = sec.SecurityId;
        }

        var delResp = await client.DeleteAsync($"/api/securityapi/{createdId}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sec = await db.Securities.FindAsync(new object[] { createdId }, default);
            Assert.NotNull(sec);
            Assert.False(sec.IsActive);
        }
    }
}
