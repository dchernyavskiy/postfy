using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.SavingPost.v1;

public record SavePost(Guid PostId) : IUpdateCommand<SavePostResponse>;

public class SavePostHandler : ICommandHandler<SavePost, SavePostResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public SavePostHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<SavePostResponse> Handle(SavePost request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var user = await _context.Users
                       .Include(x => x.SavedPosts)
                       .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);
        Guard.Against.Null(user);
        var post = await _context.Posts.FirstOrDefaultAsync(
                       x => x.Id == request.PostId,
                       cancellationToken: cancellationToken);
        Guard.Against.Null(post);

        user.SavedPosts.Add(post);

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new SavePostResponse();
    }
}
