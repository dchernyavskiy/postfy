using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Jwt;
using Postfy.Services.Network.Chats;
using Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

public record CreateMessage
    (string Text, Guid ChatId, Guid? PostId, Guid? ParentId) : ICreateCommand<CreateMessageResponse>;

public class CreateMessageHandler : ICommandHandler<CreateMessage, CreateMessageResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public CreateMessageHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CreateMessageResponse> Handle(CreateMessage request, CancellationToken cancellationToken)
    {
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");
        var message = new Message()
                      {
                          Text = request.Text,
                          PostId = request.PostId,
                          ChatId = request.ChatId,
                          ParentId = request.ParentId,
                          IsPost = request.PostId != null,
                          SenderId = userId
                      };

        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateMessageResponse(message);
    }
}
