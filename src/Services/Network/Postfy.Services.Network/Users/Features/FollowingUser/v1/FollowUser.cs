using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Users.Features.FollowingUser.v1;

public record FollowUser(Guid UserId) : ICreateCommand;

public class FollowUserHandler : ICommandHandler<FollowUser>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public FollowUserHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<Unit> Handle(FollowUser request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var followingUser = await _context.Users
                                .Include(x => x.Followers)
                                .FirstOrDefaultAsync(
                                    x => x.Id == userId,
                                    cancellationToken: cancellationToken);
        Guard.Against.Null(followingUser);

        var followedUser = await _context.Users.FirstOrDefaultAsync(
                               x => x.Id == userId,
                               cancellationToken: cancellationToken);
        Guard.Against.Null(followedUser);

        followingUser.Followers.Add(followedUser);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
