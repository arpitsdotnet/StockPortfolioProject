using System.Text.Json.Serialization;

namespace StockPortfolio.WebUI.Models;

public class StockSecurity
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
