using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Features.DeletingMessage.v1;

public record DeleteMessage(Guid Id) : IDeleteCommand<Guid>;

public class DeleteMessageHandler : ICommandHandler<DeleteMessage>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public DeleteMessageHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<Unit> Handle(DeleteMessage request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var message = await _context.Messages
                          .FirstOrDefaultAsync(
                              x => x.Chat.Users.Any(x => x.Id == userId) && x.Id == request.Id,
                              cancellationToken: cancellationToken);

        Guard.Against.Null(message);

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
