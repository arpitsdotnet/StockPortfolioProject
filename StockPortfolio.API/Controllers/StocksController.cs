using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<StockModel>> Get([FromQuery]int page = 0, int pageSize = 10, string keyword = "")
    {
        return Ok(new List<StockModel>(){
            new StockModel() { Symbol = "ABC1", Name = "ABC1 Corporation", LastPrice = 10, Change = 4, ChangePercent = 40 },
            new StockModel() { Symbol = "ABC2", Name = "ABC2 Corporation", LastPrice = 20, Change = 6, ChangePercent = 30 },
            new StockModel() { Symbol = "ABC3", Name = "ABC3 Corporation", LastPrice = 30, Change = 8, ChangePercent = 26.67M },
            new StockModel() { Symbol = "ABC4", Name = "ABC4 Corporation", LastPrice = 40, Change = 10, ChangePercent = 25 }
        });
    }
}
public class StockModel
{
    [JsonPropertyName("Symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("LastPrice")]
    public decimal LastPrice { get; set; }

    [JsonPropertyName("Change")]
    public decimal Change { get; set; }

    [JsonPropertyName("ChangePercent")]
    public decimal ChangePercent { get; set; }
}