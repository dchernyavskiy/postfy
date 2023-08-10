using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Posts;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Chats.Features.CreatingChat.v1;

public record CreateChat(ICollection<Guid> UserIds) : ICreateCommand<CreateChatResponse>;

public class CreateChatHandler : ICommandHandler<CreateChat, CreateChatResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public CreateChatHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CreateChatResponse> Handle(CreateChat request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        request.UserIds.Add(userId);
        var users = await _context.Users
                        .Where(x => request.UserIds.Contains(x.Id))
                        .ToListAsync(cancellationToken: cancellationToken);

        var chat = new Chat() {Users = users};

        await _context.Chats.AddAsync(chat, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateChatResponse(chat);
    }
}
