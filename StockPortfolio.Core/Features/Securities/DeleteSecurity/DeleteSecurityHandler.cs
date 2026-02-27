using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Features.Securities.Domain.Models;
using StockPortfolio.Core.Services.DbContexts;

namespace StockPortfolio.Core.Features.Securities.DeleteSecurity;

public sealed record DeleteSecurityRequest(int SecurityId);

public class DeleteSecurityHandler(ApplicationDbContext context)
{
    public async Task<Result<Security>> Handle(DeleteSecurityRequest request, CancellationToken cancellationToken)
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

        if (!security.IsActive)
        {
            // Already deleted/inactive - return success with current entity
            return Result<Security>.Success(security, "Security already inactive.");
        }

        security.IsActive = false;
        security.LastModifiedOn = DateTime.UtcNow;
        security.LastModifiedById = 0; // TODO: set current user id
        security.LastModifiedByName = null;

        context.Securities.Update(security);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Security>.Success(security, "Security deleted.");
    }
}
