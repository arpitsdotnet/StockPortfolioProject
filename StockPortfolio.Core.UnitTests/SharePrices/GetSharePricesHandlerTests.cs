using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.SharePrices.GetSharePrices;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.SharePrices;

public class GetSharePricesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPrices_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSharePrices_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var sp = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date, Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100, IsActive = true };
        await context.SharePriceHistories.AddAsync(sp);
        await context.SaveChangesAsync();

        var handler = new GetSharePricesHandler(context);
        var res = await handler.Handle(new GetSharePricesRequest(sec.SecurityId, null, null), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotEmpty(res.Value);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoPrices()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetSharePrices_Empty")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new GetSharePricesHandler(context);
        var res = await handler.Handle(new GetSharePricesRequest(sec.SecurityId, null, null), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
