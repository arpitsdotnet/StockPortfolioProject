using Microsoft.AspNetCore.Mvc;
using StockPortfolio.API.Extensions;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices.CreateSharePrice;
using StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;
using StockPortfolio.Core.Features.SharePrices.GetSharePrices;
using StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;
using StockPortfolio.Core.Features.SharePrices.UpdateSharePrice;

namespace StockPortfolio.API.Controllers;

/// <summary>Handles API endpoints for share price management and external data synchronization.</summary>
[ApiController]
[Route("api/[controller]")]
public class SharePricesController : ControllerBase
{
    private readonly FetchAndStoreSharePricesHandler _fetchHandler;
    private readonly CreateSharePriceHandler _createHandler;
    private readonly DeleteSharePriceHandler _deleteHandler;
    private readonly GetSharePricesHandler _getHandler;
    private readonly GetLatestSharePriceHandler _getLatestHandler;

    public SharePricesController(
        FetchAndStoreSharePricesHandler fetchHandler,
        CreateSharePriceHandler createHandler,
        DeleteSharePriceHandler deleteHandler,
        GetSharePricesHandler getHandler,
        GetLatestSharePriceHandler getLatestHandler)
    {
        _fetchHandler = fetchHandler;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
        _getHandler = getHandler;
        _getLatestHandler = getLatestHandler;
    }

    /// <summary>Fetches share price data from external API and stores in database.</summary>
    [HttpPost("fetch")]
    public async Task<IActionResult> FetchAndStore([FromBody] FetchAndStoreSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return Result<FetchAndStoreSharePricesRequest>
                .Failure(new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request required"))
                .ToActionResult();

        var result = await _fetchHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? result.ToOkResult()
            : result.ToActionResult();
    }

    /// <summary>Creates a new share price record.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSharePriceRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return Result<CreateSharePriceResponse>
                .Failure(new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request body is required"))
                .ToActionResult();

        var result = await _createHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? result.ToCreatedAtActionResult(this, nameof(GetBySecurity), new { securityId = request.SecurityId })
            : result.ToActionResult();
    }

    /// <summary>Retrieves share prices for a security within optional date range.</summary>
    [HttpGet("{securityId:int}")]
    public async Task<IActionResult> GetBySecurity(int securityId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var request = new GetSharePricesRequest(securityId, fromDate, toDate);
        var result = await _getHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? result.ToOkResult()
            : result.ToActionResult();
    }

    /// <summary>Retrieves the latest share price for a security.</summary>
    [HttpGet("{securityId:int}/latest")]
    public async Task<IActionResult> GetLatest(int securityId, CancellationToken cancellationToken)
    {
        var request = new GetLatestSharePriceRequest(securityId);
        var result = await _getLatestHandler.Handle(request, cancellationToken);
        return result.IsSuccess
            ? result.ToOkResult()
            : result.ToActionResult();
    }

    /// <summary>Deletes a share price record by ID.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.Handle(new DeleteSharePriceRequest(id), cancellationToken);
        return result.IsSuccess
            ? Ok(new ResultDto<object> { IsSuccess = true, Value = result.Value, Message = "Deleted" })
            : result.ToActionResult();
    }
}
