using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.UpdateSecurity;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.Securities;

public class UpdateSecurityHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesSecurity_WhenValid()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_UpdateSecurity_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Old", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new UpdateSecurityHandler(context);
        var req = new UpdateSecurityRequest(sec.SecurityId, Symbol: "TST2", Name: "New", Exchange: null, SecurityType: null, Currency: null, ISIN: null, Sector: null, Industry: null);

        var res = await handler.Handle(req, System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal("TST2", res.Value.Symbol);
        Assert.Equal("New", res.Value.Name);
    }

    [Fact]
    public async Task Handle_ReturnsValidation_ForInvalidId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_UpdateSecurity_Invalid")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new UpdateSecurityHandler(context);

        var res = await handler.Handle(new UpdateSecurityRequest(0, null, null, null, null, null, null, null, null), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
