using System.ComponentModel.DataAnnotations;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.Core.Features.SharePrices.Domain.Models;

public class SharePriceHistory : BaseDomain
{
    [Key]
    public int SharePriceHistoryId { get; set; }

    public int SecurityId { get; set; }

    public DateTime SeriesDate { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public long Volume { get; set; }

    // Navigation
    // public Security Security { get; set; }
}
