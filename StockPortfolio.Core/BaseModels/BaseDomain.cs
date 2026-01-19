namespace StockPortfolio.Core.BaseModels;
public abstract class BaseDomain
{
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public int CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime LastModifiedOn { get; set; }
    public int LastModifiedById { get; set; }
    public string? LastModifiedByName { get; set; }
}
