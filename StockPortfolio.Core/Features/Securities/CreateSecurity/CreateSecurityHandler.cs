using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.Securities.CreateSecurity;

public sealed record CreateSecurityRequest(
    string Symbol,
    string Name,
    string? Exchange,
    string? SecurityType,
    string? Currency,
    string? ISIN,
    string? Sector,
    string? Industry
);

public class CreateSecurityHandler(ApplicationDbContext context)
{
    public async Task<Result<Security>> Handle(CreateSecurityRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Symbol) || string.IsNullOrWhiteSpace(request.Name))
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Symbol and Name are required.");
        }

        // Normalize symbol for comparison
        var symbolNormalized = request.Symbol.Trim();

        // Check if an active security with same symbol already exists
        var exists = await context.Securities.AnyAsync(x => x.IsActive && x.Symbol == symbolNormalized, cancellationToken);
        if (exists)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.CONFLICT, "A security with the given symbol already exists.");
        }

        var security = new Security
        {
            Symbol = symbolNormalized,
            Name = request.Name.Trim(),
            Exchange = request.Exchange?.Trim(),
            SecurityType = request.SecurityType?.Trim(),
            Currency = request.Currency?.Trim(),
            ISIN = request.ISIN?.Trim(),
            Sector = request.Sector?.Trim(),
            Industry = request.Industry?.Trim(),
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedById = 0, // TODO: set current user id
            CreatedByName = null,
            LastModifiedOn = DateTime.UtcNow,
            LastModifiedById = 0,
            LastModifiedByName = null
        };

        await context.Securities.AddAsync(security, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Security>.Success(security, "Security created.");
    }
}
