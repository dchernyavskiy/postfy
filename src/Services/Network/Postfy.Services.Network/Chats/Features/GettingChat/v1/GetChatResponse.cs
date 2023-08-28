using Postfy.Services.Network.Chats.Dtos;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Posts.Dtos;

namespace Postfy.Services.Network.Chats.Features.GettingChat.v1;

public record GetChatResponse(ChatDto Chat);
