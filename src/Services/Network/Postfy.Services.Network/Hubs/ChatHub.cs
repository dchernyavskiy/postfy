using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

namespace Postfy.Services.Network.Hubs;

public class ChatHub : Hub
{
    private readonly ISender _sender;

    public ChatHub(ISender sender)
    {
        _sender = sender;
    }

    public async Task SendMessageAsync(string chatId, MessageBriefDto dto)
    {
        dto.IsWrittenByYou = false;
        await Clients.OthersInGroup(chatId).SendAsync("ReceiveMessage", dto);
    }

    public async Task JoinToChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }
}
