using Microsoft.AspNetCore.Mvc;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.API.Controllers;
[ApiController]
[Route("[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ILogger<SecurityController> _logger;
    private readonly TimeSeriesDailyHandler _symbolSearchHandler;
    private readonly ApplicationDbContext _context;

    public SecurityController(
        ILogger<SecurityController> logger,
        TimeSeriesDailyHandler symbolSearchHandler,
        ApplicationDbContext context)
    {
        _logger = logger;
        _symbolSearchHandler = symbolSearchHandler;
        _context = context;
    }

    [HttpGet(Name = "GetSecurityByKeywords")]
    public async Task<Result<List<TimeSeriesDailyResponse>>> GetSecurityByKeywords(string keywords, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{MethodName} method insert.", nameof(GetSecurityByKeywords));

        Result<List<TimeSeriesDailyResponse>> response = await _symbolSearchHandler.Handle(new TimeSeriesDailyRequest(keywords), cancellationToken);

        //List<string?> symbols = _context.Securities.Where(x => x.IsActive == true).Select(x => x.Symbol).ToList();

        //var securities = (from item in response.Value
        //                  where symbols.Contains(item.Symbol) == false
        //                  select new Security
        //                  {
        //                      Symbol = item.Symbol,
        //                      Name = item.Name,
        //                      Exchange = item.Exchange,
        //                      SecurityType = item.Type,
        //                      Currency = item.Currency,
        //                      IsActive = true,
        //                      CreatedById = 1,
        //                      CreatedOn = DateTime.Now
        //                  }).ToList();

        //await _context.Securities.AddRangeAsync(securities);
        //await _context.SaveChangesAsync();

        return response;
    }
}
