using System.Text.Json.Serialization;

namespace StockPortfolio.WebUI.Models;

public class StockSecurity
{
    private StockSecurity(string? symbol, string? name, string? exchange, string? securityType, string? currency)
    {
        Symbol = symbol;
        Name = name;
        Exchange = exchange;
        SecurityType = securityType;
        Currency = currency;
    }
    public static StockSecurity Create(string? symbol, string? name, string? exchange, string? securityType, string? currency)
    {
        return new StockSecurity(symbol, name, exchange, securityType, currency);        
    }

    [JsonPropertyName("Symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    public string? Exchange { get; set; }

    public string? SecurityType { get; set; }

    public string? Currency { get; set; }
}
