using Microsoft.AspNetCore.Mvc;
using StockPortfolio.API.Extensions;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Services.DbContexts;
using StockPortfolio.Core.Features.Securities.DeleteSecurity;
using StockPortfolio.Core.Features.Securities.UpdateSecurity;
using StockPortfolio.Core.Features.Securities.CreateSecurity;

namespace StockPortfolio.API.Controllers;

/// <summary>Handles API endpoints for security management and external data search.</summary>
[ApiController]
[Route("api/[controller]")]
public class SecurityApiController : ControllerBase
{
    private readonly ILogger<SecurityApiController> _logger;
    private readonly TimeSeriesDailyHandler _symbolSearchHandler;
    private readonly ApplicationDbContext _context;
    private readonly CreateSecurityHandler _createSecurityHandler;
    private readonly DeleteSecurityHandler _deleteSecurityHandler;
    private readonly UpdateSecurityHandler _updateSecurityHandler;

    public SecurityApiController(
        ILogger<SecurityApiController> logger,
        TimeSeriesDailyHandler symbolSearchHandler,
        ApplicationDbContext context,
        CreateSecurityHandler createSecurityHandler,
        DeleteSecurityHandler deleteSecurityHandler,
        UpdateSecurityHandler updateSecurityHandler)
    {
        _logger = logger;
        _symbolSearchHandler = symbolSearchHandler;
        _context = context;
        _createSecurityHandler = createSecurityHandler;
        _deleteSecurityHandler = deleteSecurityHandler;
        _updateSecurityHandler = updateSecurityHandler;
    }

    /// <summary>Searches for securities by keywords using external API.</summary>
    [HttpGet("search")]
    public async Task<Result<List<TimeSeriesDailyResponse>>> GetSecurityByKeywords(string keywords, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{MethodName} method called with keywords: {Keywords}", nameof(GetSecurityByKeywords), keywords);

        return await _symbolSearchHandler.Handle(new TimeSeriesDailyRequest(keywords), cancellationToken);
    }

    /// <summary>Creates a new security record.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSecurityRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create security called with symbol {Symbol}", request?.Symbol);

        if (request == null)
            return BadRequest(new ResultDto<object> 
            { 
                IsSuccess = false, 
                Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request body is required") 
            });

        var result = await _createSecurityHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? result.ToCreatedAtActionResult(this, nameof(GetById), new { id = result.Value.SecurityId })
            : result.ToActionResult();
    }

    /// <summary>Retrieves a security by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var security = await _context.Securities.FindAsync(new object[] { id }, cancellationToken);
        if (security == null)
            return NotFound(new ResultDto<object> 
            { 
                IsSuccess = false, 
                Error = new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found") 
            });

        return Ok(new ResultDto<object> { IsSuccess = true, Value = security });
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

    /// <summary>Updates an existing security record.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSecurityRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update security called for id {Id}", id);

        if (request == null)
            return BadRequest(new ResultDto<object> 
            { 
                IsSuccess = false, 
                Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request body is required") 
            });

        // Ensure request ID matches route ID
        if (request.SecurityId != id)
            request = request with { SecurityId = id };

        var result = await _updateSecurityHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? Ok(new ResultDto<object> { IsSuccess = true, Value = result.Value, Message = "Updated" })
            : result.ToActionResult();
    }
}
