using System.Text.Json.Serialization;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.Models;
internal class TimeSeries_ItemResponse
{
    [JsonPropertyName("1. open")]
    public decimal Open { get; set; }

    [JsonPropertyName("2. high")]
    public decimal High { get; set; }

    [JsonPropertyName("3. low")]
    public decimal Low { get; set; }

    [JsonPropertyName("4. close")]
    public decimal Close { get; set; }

    [JsonPropertyName("5. volumn")]
    public long Volume { get; set; }
}
