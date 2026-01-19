using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.Securities.GetSecurities;

public sealed record GetSecuritiesRequest(string Keywords, string SecurityType, string Industry);
public sealed record GetSecuritiesResponse(int SecurityId, string Symbol, string Name, string Exchange, string SecurityType, string Currency, string ISIN, string Sector, string Industry);
public class GetSecuritiesHandler
{
    private readonly ApplicationDbContext _context;

    public GetSecuritiesHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<GetSecuritiesResponse>>> Handle(GetSecuritiesRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Security> securityQuery = _context.Securities.Where(x => x.IsActive == true);

        if (string.IsNullOrWhiteSpace(request.Keywords) == false)
        {
            securityQuery = securityQuery
                .Where(x => x.Symbol!.Contains(request.Keywords, StringComparison.CurrentCultureIgnoreCase)
                        || x.Name!.Contains(request.Keywords, StringComparison.CurrentCultureIgnoreCase));
        }
        if (string.IsNullOrWhiteSpace(request.SecurityType) == false)
        {
            securityQuery = securityQuery
                .Where(x => x.SecurityType!.Equals(request.SecurityType, StringComparison.CurrentCultureIgnoreCase));
        }
        if (string.IsNullOrWhiteSpace(request.Industry) == false)
        {
            securityQuery = securityQuery
                .Where(x => x.Industry!.Equals(request.Industry, StringComparison.CurrentCultureIgnoreCase));
        }

        List<Security> securities = await securityQuery.ToListAsync(cancellationToken);
        if (securities == null || securities.Count == 0)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.NOT_FOUND, "No security record found.");
        }

        List<GetSecuritiesResponse> dataList = new();

        foreach (var item in securities)
        {
            dataList.Add(new GetSecuritiesResponse(
                SecurityId: item.SecurityId,
                Symbol: item.Symbol,
                Name: item.Name,
                Exchange: item.Exchange,
                SecurityType: item.SecurityType,
                Currency: item.Currency,
                ISIN: item.ISIN,
                Sector: item.Sector,
                Industry: item.Industry
            ));
        }

        return Result<List<GetSecuritiesResponse>>.Success(dataList);
    }
}
