using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.GetSecurities;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.Securities;

public class GetSecuritiesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsList_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSecurities_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new GetSecuritiesHandler(context);
        var res = await handler.Handle(new GetSecuritiesRequest("", "", ""), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoRecords()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSecurities_Empty")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new GetSecuritiesHandler(context);
        var res = await handler.Handle(new GetSecuritiesRequest("", "", ""), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
