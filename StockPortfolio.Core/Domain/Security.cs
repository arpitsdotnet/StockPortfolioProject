using System.ComponentModel.DataAnnotations;

namespace StockPortfolio.Core.Domain;
public class Security : BaseDomain
{
    [Key]
    public int SecurityId { get; set; }

    [Required]
    [MaxLength (20)]
    public string? Symbol { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Exchange { get; set; }

    public string? SecurityType { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; }

    [MaxLength(20)]
    public string? ISIN { get; set; }

    [MaxLength(100)]
    public string? Sector { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

}
