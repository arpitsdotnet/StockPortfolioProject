using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.SharePrices;

namespace StockPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharePricesController : ControllerBase
{
    private readonly FetchAndStoreSharePricesHandler _handler;

    public SharePricesController(FetchAndStoreSharePricesHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("fetch")]
    public async Task<IActionResult> FetchAndStore([FromBody] FetchAndStoreSharePricesRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest(new ResultDto<object> { IsSuccess = false, Error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request required") });

        var result = await _handler.Handle(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ResultDto<object> { IsSuccess = false, Error = result.Error });
        }

        return Ok(new ResultDto<int> { IsSuccess = true, Value = result.Value, Message = result.Message });
    }
}
