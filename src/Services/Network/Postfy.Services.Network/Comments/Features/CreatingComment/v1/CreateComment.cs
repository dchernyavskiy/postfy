using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Comments.Features.CreatingComment.v1;

public record CreateComment(string Text, Guid ParentId, Guid PostId) : ICreateCommand;

public class CreateCommentHandler : ICommandHandler<CreateComment>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public CreateCommentHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<Unit> Handle(CreateComment request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var comment = new Comment()
                      {
                          Text = request.Text, ParentId = request.ParentId, UserId = userId, PostId = request.PostId
                      };

        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
