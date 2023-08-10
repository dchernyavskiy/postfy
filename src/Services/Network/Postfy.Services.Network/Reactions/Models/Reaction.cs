using BuildingBlocks.Core.Domain;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Reactions.Models;

public class Reaction : Aggregate<Guid>
{
    public Reaction()
    {
        Id = Guid.NewGuid();
    }

    public bool IsLiked { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }

    public Post Post { get; set; }
    public Guid PostId { get; set; }
}
