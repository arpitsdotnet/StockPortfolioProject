using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;

namespace StockPortfolio.API.Controllers;
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

    [HttpGet(Name = "GetSecurityByKeywords")]
    public async Task<Result<List<TimeSeriesDailyResponse>>> GetSecurityByKeywords(string keywords, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{MethodName} method insert.", nameof(GetSecurityByKeywords));

        Result<List<TimeSeriesDailyResponse>> response = await _symbolSearchHandler.Handle(new TimeSeriesDailyRequest(keywords), cancellationToken);

        return response;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete security called for id {Id}", id);

        var result = await _deleteSecurityHandler.Handle(new DeleteSecurityRequest(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ResultDto<object> { IsSuccess = false, Error = new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found") });
        }

        return Ok(new ResultDto<object> { IsSuccess = true, Value = null, Message = "Deleted" });
    }
}
