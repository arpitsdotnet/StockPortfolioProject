namespace StockPortfolio.WebUI.Models.BaseModels;

public sealed class ResultDto<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }
    public Error? Error { get; set; }
}

