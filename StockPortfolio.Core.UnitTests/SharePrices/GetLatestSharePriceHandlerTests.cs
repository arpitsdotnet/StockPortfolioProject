using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.Securities.Domain.Models;

namespace StockPortfolio.Core.UnitTests.SharePrices;

public class GetLatestSharePriceHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsLatest_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetLatestSharePrice_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var sp1 = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date.AddDays(-1), Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100, IsActive = true };
        var sp2 = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date, Open = 2, High = 3, Low = 1, Close = 2.5m, Volume = 200, IsActive = true };
        await context.SharePriceHistories.AddRangeAsync(sp1, sp2);
        await context.SaveChangesAsync();

        var handler = new GetLatestSharePriceHandler(context);
        var res = await handler.Handle(new GetLatestSharePriceRequest(sec.SecurityId), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal(2.5m, res.Value.Close);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoPrices()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_GetLatestSharePrice_Empty")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new GetLatestSharePriceHandler(context);
        var res = await handler.Handle(new GetLatestSharePriceRequest(9999), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
    }
}
