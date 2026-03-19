using System.Text.Json.Serialization;

namespace StockPortfolio.WebUI.Models;

public class SharePrice
{
    public SharePrice()
    {

    }
    public SharePrice(int? securityId, DateTime? seriesDate, decimal? open, decimal? high, decimal? low, decimal? close, decimal? volume)
    {
        SecurityId = securityId;
        SeriesDate = seriesDate;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    public static SharePrice Create(int? securityId, DateTime? seriesDate, decimal? open, decimal? high, decimal? low, decimal? close, decimal? volume)
    {
        return new SharePrice(securityId, seriesDate, open, high, low, close, volume);
    }

    [JsonPropertyName("SecurityId")]
    public int? SecurityId { get; set; }

    [JsonPropertyName("SeriesDate")]
    public DateTime? SeriesDate { get; set; }

    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public decimal? Volume { get; set; }
}
