using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internal;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using MediatR.Pipeline;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats;
using Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;
using Postfy.Services.Network.Hubs;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Notifications.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

public record CreateMessage
    (string Text, Guid ChatId, Guid? PostId, Guid? ParentId) : ICreateCommand<CreateMessageResponse>;

public class CreateMessageHandler : ICommandHandler<CreateMessage, CreateMessageResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMapper _mapper;

    public CreateMessageHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper,
        IHubContext<NotificationHub> hubContext
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<CreateMessageResponse> Handle(CreateMessage request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var sender = await _context.Users.FirstOrDefaultAsync(
                         x => x.Id == userId,
                         cancellationToken: cancellationToken);
        var message = new Message()
                      {
                          Text = request.Text,
                          PostId = request.PostId,
                          ChatId = request.ChatId,
                          ParentId = request.ParentId,
                          IsPost = request.PostId != null,
                          SenderId = userId
                      };
        // var message = Message.Create(
        //     request.Text,
        //     request.PostId,
        //     request.ChatId,
        //     request.ParentId,
        //     request.PostId != null,
        //     userId);

        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await _context.Messages
                      .Include(x => x.Sender)
                      .Include(x => x.Post)
                      .ProjectTo<MessageBriefDto>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                      .FirstAsync(x => x.Id == message.Id, cancellationToken: cancellationToken);

        var users = await _context.Chats.Where(x => x.Id == request.ChatId)
                        .SelectMany(x => x.Users)
                        .Where(x => x.Id != userId)
                        .ToListAsync(cancellationToken: cancellationToken);
        var notification = new Notification() {Text = $"@{sender.ProfileName} sent you message."};
        foreach (var user in users)
        {
            if (user.NotificationSettings?.SomeoneSendMessage ?? true)
            {
                await _hubContext.Clients.Groups(user.Id.ToString())
                    .SendAsync(
                        "ReceiveNotification",
                        notification,
                        cancellationToken: cancellationToken);
            }
        }

        return new CreateMessageResponse(dto);
    }
}
