using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.Securities.UpdateSecurity;

public sealed record UpdateSecurityRequest(
    int SecurityId,
    string? Symbol,
    string? Name,
    string? Exchange,
    string? SecurityType,
    string? Currency,
    string? ISIN,
    string? Sector,
    string? Industry
);

public class UpdateSecurityHandler(ApplicationDbContext context)
{
    public async Task<Result<Security>> Handle(UpdateSecurityRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.SecurityId <= 0)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Invalid security id.");
        }

        var security = await context.Securities.FirstOrDefaultAsync(x => x.SecurityId == request.SecurityId, cancellationToken);
        if (security == null)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Security not found.");
        }

        // Update only allowed fields when provided (null means no change)
        if (!string.IsNullOrWhiteSpace(request.Symbol)) security.Symbol = request.Symbol;
        if (!string.IsNullOrWhiteSpace(request.Name)) security.Name = request.Name;
        if (request.Exchange != null) security.Exchange = request.Exchange;
        if (request.SecurityType != null) security.SecurityType = request.SecurityType;
        if (request.Currency != null) security.Currency = request.Currency;
        if (request.ISIN != null) security.ISIN = request.ISIN;
        if (request.Sector != null) security.Sector = request.Sector;
        if (request.Industry != null) security.Industry = request.Industry;

        security.LastModifiedOn = DateTime.UtcNow;
        security.LastModifiedById = 0; // TODO: set current user id
        security.LastModifiedByName = null;

        context.Securities.Update(security);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Security>.Success(security, "Security updated.");
    }
}
