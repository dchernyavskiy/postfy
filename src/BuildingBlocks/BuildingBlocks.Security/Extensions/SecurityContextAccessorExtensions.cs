using Ardalis.GuardClauses;
using BuildingBlocks.Security.Jwt;

namespace BuildingBlocks.Security.Extensions;

public static class SecurityContextAccessorExtensions
{
    public static Guid GetIdAsGuid(this ISecurityContextAccessor securityContextAccessor)
    {
        var userIdStr = securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");
        return userId;
    }
}
