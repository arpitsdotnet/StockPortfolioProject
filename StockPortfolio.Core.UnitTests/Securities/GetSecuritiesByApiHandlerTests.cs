using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.GetSecuritiesByApi;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.Securities;

public class GetSecuritiesByApiHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsList_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSecuritiesByApi_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new GetSecuritiesByApiHandler(context);
        var res = await handler.Handle(new GetSecuritiesByApiRequest("", "", ""), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoRecords()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSecuritiesByApi_Empty")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new GetSecuritiesByApiHandler(context);
        var res = await handler.Handle(new GetSecuritiesByApiRequest("", "", ""), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
