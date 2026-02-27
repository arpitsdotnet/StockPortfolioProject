using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.CreateSecurity;

namespace StockPortfolio.Core.UnitTests;

public class CreateSecurityHandlerTests
{
    [Fact]
    public async Task Handle_CreatesSecurity_WhenValid()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSecurity")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new CreateSecurityHandler(context);

        var request = new CreateSecurityRequest("TST", "Test Corp", "NYSE", "Equity", "USD", null, null, null);

        var result = await handler.Handle(request, System.Threading.CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("TST", result.Value.Symbol);
    }
}
