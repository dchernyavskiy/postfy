using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Exceptions.Application;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Comments.Features.DeletingComment.v1;

public record DeleteComment(Guid Id) : IDeleteCommand<Guid>;

public class DeleteCommentHandler : ICommandHandler<DeleteComment>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public async Task<Unit> Handle(DeleteComment request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var comment = await _context.Comments
                          .FirstOrDefaultAsync(
                              x => x.Id == request.Id && x.UserId == userId,
                              cancellationToken: cancellationToken);

        Guard.Against.NotFound(comment, new PostNotFoundException(request.Id));

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
