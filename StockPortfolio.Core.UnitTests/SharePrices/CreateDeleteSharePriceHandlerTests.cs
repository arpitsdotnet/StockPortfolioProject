using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.SharePrices.CreateSharePrice;
using StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;
using StockPortfolio.Core.Features.SharePrices.Domain.Models;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.BaseModels;
using System.Threading;

namespace StockPortfolio.Core.UnitTests.SharePrices;

public class CreateDeleteSharePriceHandlerTests
{
    [Fact]
    public async Task Create_AddsPrice_WhenValid()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new CreateSharePriceHandler(context);
        var req = new CreateSharePriceRequest(sec.SecurityId, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100);
        var res = await handler.Handle(req, System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task Create_ReturnsValidation_ForInvalidSecurityId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_InvalidId")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new CreateSharePriceHandler(context);

        var res = await handler.Handle(new CreateSharePriceRequest(0, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.BAD_REQUEST, res.Error.Code);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenSecurityMissing()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_SecurityMissing")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new CreateSharePriceHandler(context);

        var res = await handler.Handle(new CreateSharePriceRequest(9999, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.NOT_FOUND, res.Error.Code);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenDuplicateDate()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_Conflict")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var sp = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date, Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100, IsActive = true };
        await context.SharePriceHistories.AddAsync(sp);
        await context.SaveChangesAsync();

        var handler = new CreateSharePriceHandler(context);
        var req = new CreateSharePriceRequest(sec.SecurityId, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100);
        var res = await handler.Handle(req, System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.CONFLICT, res.Error.Code);
    }

    [Fact]
    public async Task Create_ReturnsValidation_ForNegativeSecurityId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_NegativeId")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new CreateSharePriceHandler(context);

        var res = await handler.Handle(new CreateSharePriceRequest(-5, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100), CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.BAD_REQUEST, res.Error.Code);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenSecurityInactive()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_CreateSharePrice_SecurityInactive")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = false, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var handler = new CreateSharePriceHandler(context);
        var res = await handler.Handle(new CreateSharePriceRequest(sec.SecurityId, System.DateTime.UtcNow.Date, 1, 2, 0.5m, 1.5m, 100), CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.NOT_FOUND, res.Error.Code);
    }

    [Fact]
    public async Task Delete_MarksInactive_WhenExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSharePrice_Success")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var sp = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date, Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100, IsActive = true };
        await context.SharePriceHistories.AddAsync(sp);
        await context.SaveChangesAsync();

        var handler = new DeleteSharePriceHandler(context);
        var res = await handler.Handle(new DeleteSharePriceRequest(sp.SharePriceHistoryId), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.True(res.Value.IsDeleted);
    }

    [Fact]
    public async Task Delete_ReturnsAlreadyInactive_WhenAlreadyInactive()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSharePrice_AlreadyInactive")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var sec = new Security { Symbol = "TST", Name = "Test", IsActive = true, CreatedOn = System.DateTime.UtcNow };
        await context.Securities.AddAsync(sec);
        await context.SaveChangesAsync();

        var sp = new SharePriceHistory { SecurityId = sec.SecurityId, SeriesDate = System.DateTime.UtcNow.Date, Open = 1, High = 2, Low = 0.5m, Close = 1.5m, Volume = 100, IsActive = false };
        await context.SharePriceHistories.AddAsync(sp);
        await context.SaveChangesAsync();

        var handler = new DeleteSharePriceHandler(context);
        var res = await handler.Handle(new DeleteSharePriceRequest(sp.SharePriceHistoryId), System.Threading.CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.True(res.Value.IsDeleted);
    }

    [Fact]
    public async Task Delete_ReturnsValidation_ForInvalidId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSharePrice_InvalidId")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new DeleteSharePriceHandler(context);

        var res = await handler.Handle(new DeleteSharePriceRequest(0), System.Threading.CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.BAD_REQUEST, res.Error.Code);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSharePrice_NotFound")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new DeleteSharePriceHandler(context);

        var res = await handler.Handle(new DeleteSharePriceRequest(12345), CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.NOT_FOUND, res.Error.Code);
    }

    [Fact]
    public async Task Delete_ReturnsValidation_ForNegativeId()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_DeleteSharePrice_NegativeId")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var handler = new DeleteSharePriceHandler(context);

        var res = await handler.Handle(new DeleteSharePriceRequest(-10), CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal(ErrorCode.BAD_REQUEST, res.Error.Code);
    }
}
