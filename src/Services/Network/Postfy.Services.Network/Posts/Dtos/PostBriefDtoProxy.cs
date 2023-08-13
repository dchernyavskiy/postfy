using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Posts.Dtos;

public record PostBriefDtoProxy() : IMapWith<Post>
{
    public Guid Id { get; set; }
    public Guid CurrentUserId { get; set; }
    public string Caption { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Message? Message { get; set; }
    public Guid? MessageId { get; set; }


    public ICollection<Media> Medias { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Reaction> Reactions { get; set; }
    public ICollection<User> Savers { get; set; }
}
