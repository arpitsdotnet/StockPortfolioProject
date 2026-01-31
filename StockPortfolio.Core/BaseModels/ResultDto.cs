using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolio.Core.BaseModels;
public sealed class ResultDto<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }
    public Error? Error { get; set; }
}
