using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Hubs;
using Postfy.Services.Network.Notifications.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Users.Features.FollowingUser.v1;

public record FollowUser(Guid UserId) : ICreateCommand;

public class FollowUserHandler : ICommandHandler<FollowUser>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IHubContext<NotificationHub> _hubContext;

    public FollowUserHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IHubContext<NotificationHub> hubContext
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _hubContext = hubContext;
    }

    public async Task<Unit> Handle(FollowUser request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var follower = await _context.Users
                           .Include(x => x.Followings)
                           .FirstOrDefaultAsync(
                               x => x.Id == userId,
                               cancellationToken: cancellationToken);
        Guard.Against.Null(follower);

        var following = await _context.Users.FirstOrDefaultAsync(
                            x => x.Id == request.UserId,
                            cancellationToken: cancellationToken);
        Guard.Against.Null(following);

        follower.Followings.Add(following);
        await _context.SaveChangesAsync(cancellationToken);

        if (follower.NotificationSettings?.SomeoneFollowed ?? true)
        {
            var notification = new Notification() {Text = $"@{follower.ProfileName} followed you."};
            await _hubContext.Clients.Groups(following.Id.ToString())
                .SendAsync(
                    "ReceiveNotification",
                    notification,
                    cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}
