using BuildingBlocks.Core.Domain;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Comments.Models;

public class Comment : Aggregate<Guid>
{
    public Comment()
    {
        Id = Guid.NewGuid();
    }

    public string Text { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid PostId { get; set; }
    public Post Post { get; set; }

    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Children { get; set; }
}
