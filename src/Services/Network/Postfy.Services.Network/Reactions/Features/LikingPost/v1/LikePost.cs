using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Hubs;
using Postfy.Services.Network.Notifications.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Reactions.Features.LikingPost.v1;

public record LikePost(Guid PostId) : IUpdateCommand;

public class LikePostHandler : ICommandHandler<LikePost>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMediator _mediator;

    public LikePostHandler(
        ISecurityContextAccessor securityContextAccessor,
        INetworkDbContext context,
        IHubContext<NotificationHub> hubContext,
        IMediator mediator
    )
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
        _hubContext = hubContext;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(LikePost request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var reaction =
            await _context.Reactions
                .FirstOrDefaultAsync(
                    x => x.PostId == request.PostId && x.UserId == userId,
                    cancellationToken: cancellationToken);
        if (reaction == null)
        {
            reaction = new Reaction() {IsLiked = true, PostId = request.PostId, UserId = userId};
            await _context.Reactions.AddAsync(reaction, cancellationToken);
        }
        else
        {
            reaction.IsLiked = !reaction.IsLiked;
            _context.Reactions.Update(reaction);
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (reaction.IsLiked)
        {
            var post = await _context.Posts
                           .Include(x => x.User)
                           .FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken: cancellationToken);
            if (post?.User?.NotificationSettings?.SomeoneLikedPost ?? true)
            {
                var liker = await _context.Users.FirstOrDefaultAsync(
                                x => x.Id == userId,
                                cancellationToken: cancellationToken);
                var notification = new Notification() {Text = $"@{liker.ProfileName} liked your post."};
                await _hubContext.Clients.Groups(post?.UserId.ToString())
                    .SendAsync(
                        "ReceiveNotification",
                        notification,
                        cancellationToken: cancellationToken);
            }
        }

        return Unit.Value;
    }
}
