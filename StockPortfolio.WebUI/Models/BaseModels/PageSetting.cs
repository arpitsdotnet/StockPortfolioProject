namespace StockPortfolio.WebUI.Models.BaseModels;

public class PageSetting(int page = 0, int pageSize = 10)
{
    public int PageNum { get; private set; } = page;
    public int PageSize { get; private set; } = pageSize;
}
