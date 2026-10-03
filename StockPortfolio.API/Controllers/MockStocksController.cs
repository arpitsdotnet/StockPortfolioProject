using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.Stocks.Domain;

namespace StockPortfolio.API.Controllers;

/// <summary>Mock controller providing sample stock data for testing and demonstration purposes.</summary>
[Route("api/[controller]")]
[ApiController]
public class MockStocksController : ControllerBase
{
    private static List<StockModel> _stocks;

    public MockStocksController()
    {
        _stocks = [
            new() { StockId=1, Symbol = "ABC1", Name = "ABC1 Corporation", LastPrice = 10, Change = 4, ChangePercent = 40 },
            new() { StockId=2, Symbol = "ABC2", Name = "ABC2 Corporation", LastPrice = 20, Change = 6, ChangePercent = 30 },
            new() { StockId=3, Symbol = "ABC3", Name = "ABC3 Corporation", LastPrice = 30, Change = 8, ChangePercent = 26.67M },
            new() { StockId=4, Symbol = "ABC4", Name = "ABC4 Corporation", LastPrice = 40, Change = 10, ChangePercent = 25 }
        ];
    }

    /// <summary>Retrieves a paginated list of mock stock data.</summary>
    [HttpGet]
    public ActionResult<ResultDto<List<StockModel>>> Get([FromQuery] int page = 1, int pageSize = 10, string keyword = "")
    {
        // TODO : Returns hardcoded sample data; in production, would query database
        var result = _stocks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            result = result.Where(s => 
                s.Symbol.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        var recordCount = result.Count();

        var paginatedResult = result
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();


        return Ok(new ResultDto<List<StockModel>> { IsSuccess = true, Value = paginatedResult, RecordCount = recordCount, Message = "Success." });
    }

    /// <summary>Adds a new stock record (mock implementation).</summary>
    [HttpPost]
    public ActionResult<Result<StockModel>> AddStock([FromBody] StockModel model)
    {
        // TODO: Implement proper validation, duplicate check, and database persistence

        var nextStockId = _stocks.Max(s => s.StockId) + 1;

        model.StockId = nextStockId;
        _stocks.Add(model);

        return Ok(model);
    }
}
