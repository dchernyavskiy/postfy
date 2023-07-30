using BuildingBlocks.Core.Domain;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Chats.Models;

public class Chat : Aggregate<Guid>
{
    public Chat()
    {
        Id = Guid.NewGuid();
    }

    public ICollection<User> Users { get; set; }
    public ICollection<Message> Messages { get; set; }
}
