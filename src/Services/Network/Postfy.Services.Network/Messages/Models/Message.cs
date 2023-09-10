using System.Collections;
using BuildingBlocks.Core.Domain;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Messages.Features.CreatingMessage.v1.Events;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Messages.Models;

public class Message : Aggregate<Guid>
{
    public Message()
    {
        Id = Guid.NewGuid();
    }

    public static Message Create(
        string text,
        Guid? postId,
        Guid chatId,
        Guid? parentId,
        bool isPost,
        Guid senderId
    )
    {
        var message = new Message()
                      {
                          Text = text,
                          PostId = postId,
                          ChatId = chatId,
                          ParentId = parentId,
                          IsPost = isPost,
                          SenderId = senderId
                      };
        message.AddDomainEvents(new MessageCreated(message));
        return message;
    }

    public string Text { get; set; }
    public bool IsPost { get; set; }

    public Post? Post { get; set; }
    public Guid? PostId { get; set; }

    public User Sender { get; set; }
    public Guid SenderId { get; set; }

    public Message? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<Message> Children { get; set; }

    public Chat Chat { get; set; }
    public Guid ChatId { get; set; }

    public ICollection<Media> Medias { get; set; }
}
