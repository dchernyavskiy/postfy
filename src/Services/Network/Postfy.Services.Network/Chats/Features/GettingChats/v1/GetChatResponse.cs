using Postfy.Services.Network.Chats.Dtos;
using Postfy.Services.Network.Chats.Models;

namespace Postfy.Services.Network.Chats.Features.GettingChats.v1;

public record GetChatsResponse(ICollection<ChatBriefDto> Chat);
