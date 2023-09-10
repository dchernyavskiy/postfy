using BuildingBlocks.Abstractions.CQRS.Events.Internal;
using BuildingBlocks.Core.CQRS.Events.Internal;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Hubs;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Notifications.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1.Events;

public record MessageCreated(Message Message) : DomainEvent;

public class MessageCreatedHandler : IDomainEventHandler<MessageCreated>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IHubContext<NotificationHub> _hubContext;

    public MessageCreatedHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IHubContext<NotificationHub> hubContext
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _hubContext = hubContext;
    }

    public async Task Handle(MessageCreated notification, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var sender = await _context.Users.FirstOrDefaultAsync(
                         x => x.Id == userId,
                         cancellationToken: cancellationToken);
        var users = await _context.Chats.Where(x => x.Id == notification.Message.ChatId)
                        .SelectMany(x => x.Users)
                        .Where(x => x.Id != userId)
                        .ToListAsync(cancellationToken: cancellationToken);
        var note = new Notification() {Text = $"@{sender.ProfileName} send you message."};
        foreach (var user in users)
        {
            if (user.NotificationSettings?.SomeoneSendMessage ?? true)
            {
                await _hubContext.Clients.Groups(user.Id.ToString())
                    .SendAsync(
                        "ReceiveNotification",
                        note,
                        cancellationToken: cancellationToken);
            }
        }
    }
}
