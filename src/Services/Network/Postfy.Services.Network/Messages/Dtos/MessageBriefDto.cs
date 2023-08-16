using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Messages.Dtos;

public class MessageBriefDto : IMapWith<Message>
{
    public Guid Id { get; set; }
    public string Text { get; set; }

    public bool IsPost { get; set; }
    public PostBriefDto? Post { get; set; }
    public UserBriefDto User { get; set; }
    public MessageBriefDto? Parent { get; set; }
}
