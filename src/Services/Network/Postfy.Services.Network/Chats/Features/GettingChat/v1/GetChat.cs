using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Posts;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Chats.Features.GettingChat.v1;

public record GetChat(ICollection<Guid> UserIds) : ICreateCommand<GetChatResponse>;

public class GetChatHandler : ICommandHandler<GetChat, GetChatResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public GetChatHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<GetChatResponse> Handle(GetChat request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        request.UserIds.Add(userId);
        var users = await _context.Users
                        .Where(x => request.UserIds.Contains(x.Id))
                        .ToListAsync(cancellationToken: cancellationToken);

        var userIds = users.Select(x => x.Id);
        var chat = await _context.Chats
                       .Include(x => x.Messages)
                       .ThenInclude(x => x.Sender)
                       .FirstOrDefaultAsync(
                           x => x.Users.All(u => userIds.Contains(u.Id)),
                           cancellationToken: cancellationToken);

        if (chat == null)
        {
            chat = new Chat() {Users = users};
            await _context.Chats.AddAsync(chat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new GetChatResponse(chat);
    }
}
