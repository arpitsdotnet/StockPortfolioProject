using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.Securities;

public class DeleteSecurityHandlerTests
{
    [Fact]
    public async Task Handle_DeletesSecurity_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSecurity_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new DeleteSecurityHandler(context);
        var res = await handler.Handle(new DeleteSecurityRequest(sec.SecurityId), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.False(res.Value.IsActive);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenMissing()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSecurity_NotFound")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new DeleteSecurityHandler(context);
        var res = await handler.Handle(new DeleteSecurityRequest(9999), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
