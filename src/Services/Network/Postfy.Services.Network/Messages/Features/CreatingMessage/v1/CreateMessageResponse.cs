using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Models;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

public record CreateMessageResponse(MessageBriefDto Message);
