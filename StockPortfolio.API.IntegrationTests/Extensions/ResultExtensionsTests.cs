using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StockPortfolio.API.Extensions;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.API.IntegrationTests.Extensions;

/// <summary>Unit tests for ResultExtensions mapping methods.</summary>
public class ResultExtensionsTests
{
    [Fact]
    public void ToActionResult_WithSuccessResult_ReturnsOkObjectResult()
    {
        // Arrange
        var result = Result<object>.Success("test");

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        
        var resultDto = Assert.IsType<ResultDto<object>>(okResult.Value);
        Assert.True(resultDto.IsSuccess);
    }

    [Fact]
    public void ToActionResult_WithBadRequestError_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid input");
        var result = Result<object>.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        
        var resultDto = Assert.IsType<ResultDto<object>>(badRequestResult.Value);
        Assert.False(resultDto.IsSuccess);
        Assert.Equal(ErrorCode.BAD_REQUEST, resultDto.Error.Code);
    }

    [Fact]
    public void ToActionResult_WithNotFoundError_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var error = new Error(ErrorType.VALIDATION, ErrorCode.NOT_FOUND, "Entity not found");
        var result = Result<object>.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        
        var resultDto = Assert.IsType<ResultDto<object>>(notFoundResult.Value);
        Assert.False(resultDto.IsSuccess);
        Assert.Equal(ErrorCode.NOT_FOUND, resultDto.Error.Code);
    }

    [Fact]
    public void ToActionResult_WithConflictError_ReturnsConflictObjectResult()
    {
        // Arrange
        var error = new Error(ErrorType.VALIDATION, ErrorCode.CONFLICT, "Resource already exists");
        var result = Result<object>.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        
        var resultDto = Assert.IsType<ResultDto<object>>(conflictResult.Value);
        Assert.False(resultDto.IsSuccess);
        Assert.Equal(ErrorCode.CONFLICT, resultDto.Error.Code);
    }

    [Fact]
    public void ToActionResult_WithUnknownError_ReturnsInternalServerErrorObjectResult()
    {
        // Arrange
        var error = new Error(ErrorType.FAILURE, ErrorCode.INTERNAL_SERVER_ERROR, "Unexpected error");
        var result = Result<object>.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }

    [Fact]
    public void ToCreatedAtActionResult_WithSuccessResult_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var result = Result<object>.Success("data");
        var controller = new MockController();

        // Act
        var actionResult = result.ToCreatedAtActionResult(controller, "GetById", new { id = 1 });

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal("GetById", createdResult.ActionName);
        
        var resultDto = Assert.IsType<ResultDto<object>>(createdResult.Value);
        Assert.True(resultDto.IsSuccess);
    }

    [Fact]
    public void ToCreatedAtActionResult_WithFailureResult_ReturnsErrorActionResult()
    {
        // Arrange
        var error = new Error(ErrorType.VALIDATION, ErrorCode.NOT_FOUND, "Entity not found");
        var result = Result<object>.Failure(error);
        var controller = new MockController();

        // Act
        var actionResult = result.ToCreatedAtActionResult(controller, "GetById", new { id = 1 });

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public void ToOkResult_WithSuccessResult_ReturnsOkObjectResult()
    {
        // Arrange
        var result = Result<object>.Success("data");

        // Act
        var actionResult = result.ToOkResult();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        
        var resultDto = Assert.IsType<ResultDto<object>>(okResult.Value);
        Assert.True(resultDto.IsSuccess);
    }

    [Fact]
    public void ToOkResult_WithFailureResult_ReturnsErrorActionResult()
    {
        // Arrange
        var error = new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid data");
        var result = Result<object>.Failure(error);

        // Act
        var actionResult = result.ToOkResult();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    /// <summary>Mock controller for testing extension methods that require ControllerBase.</summary>
    private class MockController : ControllerBase
    {
    }
}
