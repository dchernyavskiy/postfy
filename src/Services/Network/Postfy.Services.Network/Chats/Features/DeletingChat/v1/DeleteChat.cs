using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Chats.Features.DeletingChat.v1;

public record DeleteChat(Guid Id) : IDeleteCommand<Guid>;

public class DeleteChatHandler : ICommandHandler<DeleteChat>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public DeleteChatHandler(ISecurityContextAccessor securityContextAccessor, INetworkDbContext context)
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
    }

    public async Task<Unit> Handle(DeleteChat request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var chat = await _context.Chats.FirstOrDefaultAsync(
                       x => x.Id == request.Id && x.Users.Any(u => u.Id == userId),
                       cancellationToken: cancellationToken);
        Guard.Against.Null(chat);

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
