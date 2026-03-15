using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.API.Extensions;

/// <summary>Extension methods for converting Result<T> to IActionResult with appropriate HTTP status codes.</summary>
public static class ResultExtensions
{
    /// <summary>Maps Result<T> to IActionResult based on success/failure and error code.</summary>
    public static IActionResult ToActionResult<T>(this Result<T> result) where T : class
    {
        if (result.IsSuccess)
            return new OkObjectResult(new ResultDto<T> { IsSuccess = true, Value = result.Value });

        return result.Error.Code switch
        {
            ErrorCode.BAD_REQUEST => new BadRequestObjectResult(
                new ResultDto<object> { IsSuccess = false, Error = result.Error }),
            ErrorCode.NOT_FOUND => new NotFoundObjectResult(
                new ResultDto<object> { IsSuccess = false, Error = result.Error }),
            ErrorCode.CONFLICT => new ConflictObjectResult(
                new ResultDto<object> { IsSuccess = false, Error = result.Error }),
            _ => new ObjectResult(new ResultDto<object> { IsSuccess = false, Error = result.Error })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }

    /// <summary>Maps Result<T> to CreatedAtAction response (201 Created) for POST operations.</summary>
    public static IActionResult ToCreatedAtActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string actionName,
        object? routeValues = null) where T : class
    {
        if (!result.IsSuccess)
            return result.ToActionResult();

        return new CreatedAtActionResult(
            actionName,
            controller.ControllerContext.ActionDescriptor.ControllerName,
            routeValues,
            new ResultDto<T> { IsSuccess = true, Value = result.Value, Message = result.Message });
    }

    /// <summary>Maps Result<T> to OkObjectResult (200 OK) response for GET operations.</summary>
    public static IActionResult ToOkResult<T>(this Result<T> result) where T : class
    {
        if (!result.IsSuccess)
            return result.ToActionResult();

        return new OkObjectResult(new ResultDto<T> { IsSuccess = true, Value = result.Value });
    }
}
