using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Reactions.Features.LikingPost.v1;

public record LikePost(Guid PostId) : IUpdateCommand;

public class LikePostHandler : ICommandHandler<LikePost>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public LikePostHandler(ISecurityContextAccessor securityContextAccessor, INetworkDbContext context)
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
    }

    public async Task<Unit> Handle(LikePost request, CancellationToken cancellationToken)
    {
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");

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

        return Unit.Value;
    }
}
