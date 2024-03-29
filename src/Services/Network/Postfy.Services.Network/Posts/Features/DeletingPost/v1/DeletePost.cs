using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Exceptions.Application;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.DeletingPost.v1;

public record DeletePost(Guid Id) : IDeleteCommand<Guid>;

public class DeletePostHandler : ICommandHandler<DeletePost>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public async Task<Unit> Handle(DeletePost request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var post = await _context.Posts
                       .FirstOrDefaultAsync(
                           x => x.Id == request.Id && x.UserId == userId,
                           cancellationToken: cancellationToken);

        Guard.Against.NotFound(post, new PostNotFoundException(request.Id));

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
