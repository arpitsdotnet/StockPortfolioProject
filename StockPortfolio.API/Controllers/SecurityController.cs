using Microsoft.AspNetCore.Mvc;
using StockPortfolio.API.Extensions;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;

namespace StockPortfolio.API.Controllers;

/// <summary>Handles security-related operations via HTTP endpoints.</summary>
[ApiController]
[Route("[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ILogger<SecurityController> _logger;
    private readonly TimeSeriesDailyHandler _symbolSearchHandler;
    private readonly ApplicationDbContext _context;
    private readonly DeleteSecurityHandler _deleteSecurityHandler;

    public SecurityController(
        ILogger<SecurityController> logger,
        TimeSeriesDailyHandler symbolSearchHandler,
        ApplicationDbContext context,
        DeleteSecurityHandler deleteSecurityHandler)
    {
        _logger = logger;
        _symbolSearchHandler = symbolSearchHandler;
        _context = context;
        _deleteSecurityHandler = deleteSecurityHandler;
    }

    /// <summary>Searches for securities by keywords using external API.</summary>
    [HttpGet(Name = "GetSecurityByKeywords")]
    public async Task<Result<List<TimeSeriesDailyResponse>>> GetSecurityByKeywords(string keywords, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{MethodName} method called with keywords: {Keywords}", nameof(GetSecurityByKeywords), keywords);

        return await _symbolSearchHandler.Handle(new TimeSeriesDailyRequest(keywords), cancellationToken);
    }

    /// <summary>Deletes a security by ID.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete security called for id {Id}", id);

        var result = await _deleteSecurityHandler.Handle(new DeleteSecurityRequest(id), cancellationToken);
        return result.IsSuccess
            ? Ok(new ResultDto<object> { IsSuccess = true, Value = null, Message = "Deleted" })
            : result.ToActionResult();
    }
}
