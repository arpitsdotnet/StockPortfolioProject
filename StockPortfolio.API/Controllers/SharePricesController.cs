using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices;
using StockPortfolio.Core.Features.SharePrices.CreateSharePrice;
using StockPortfolio.Core.Features.SharePrices.DeleteSharePrice;
using StockPortfolio.Core.Features.SharePrices.GetSharePrices;
using StockPortfolio.Core.Features.SharePrices.GetLatestSharePrice;

namespace StockPortfolio.API.Controllers;

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

    [HttpPost("fetch")]
    public async Task<IActionResult> FetchAndStore([FromBody] FetchAndStoreSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest(new ResultDto<object> { IsSuccess = false, Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request required") });

        var result = await _fetchHandler.Handle(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return Ok(new ResultDto<int> { IsSuccess = true, Value = result.Value, Message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSharePriceRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest(new ResultDto<object> { IsSuccess = false, Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request body is required") });

        var result = await _createHandler.Handle(request, cancellationToken);
        if (result.IsFailure)
        {
            var code = result.Error.Code;
            if (code == ErrorCode.BAD_REQUEST)
                return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
            if (code == ErrorCode.NOT_FOUND)
                return NotFound(new ResultDto<object> { IsSuccess = false, Error = result.Error });
            if (code == ErrorCode.CONFLICT)
                return Conflict(new ResultDto<object> { IsSuccess = false, Error = result.Error });

            return StatusCode(500, new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return CreatedAtAction(nameof(GetBySecurity), new { securityId = request.SecurityId }, new ResultDto<object> { IsSuccess = true, Value = result.Value, Message = "Share price created." });
    }

    [HttpGet("{securityId:int}")]
    public async Task<IActionResult> GetBySecurity(int securityId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var request = new GetSharePricesRequest(securityId, fromDate, toDate);
        var result = await _getHandler.Handle(request, cancellationToken);
        if (result.IsFailure)
        {
            var code = result.Error.Code;
            if (code == ErrorCode.BAD_REQUEST)
                return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
            if (code == ErrorCode.NOT_FOUND)
                return NotFound(new ResultDto<object> { IsSuccess = false, Error = result.Error });

            return StatusCode(500, new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return Ok(new ResultDto<object> { IsSuccess = true, Value = result.Value });
    }

    [HttpGet("{securityId:int}/latest")]
    public async Task<IActionResult> GetLatest(int securityId, CancellationToken cancellationToken)
    {
        var request = new GetLatestSharePriceRequest(securityId);
        var result = await _getLatestHandler.Handle(request, cancellationToken);
        if (result.IsFailure)
        {
            var code = result.Error.Code;
            if (code == ErrorCode.BAD_REQUEST)
                return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
            if (code == ErrorCode.NOT_FOUND)
                return NotFound(new ResultDto<object> { IsSuccess = false, Error = result.Error });

            return StatusCode(500, new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return Ok(new ResultDto<object> { IsSuccess = true, Value = result.Value });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.Handle(new DeleteSharePriceRequest(id), cancellationToken);
        if (result.IsFailure)
        {
            var code = result.Error.Code;
            if (code == ErrorCode.BAD_REQUEST)
                return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
            if (code == ErrorCode.NOT_FOUND)
                return NotFound(new ResultDto<object> { IsSuccess = false, Error = result.Error });

            return StatusCode(500, new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return Ok(new ResultDto<object> { IsSuccess = true, Value = result.Value, Message = "Deleted" });
    }
}
