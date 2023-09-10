using BuildingBlocks.Core.Domain;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Users.Models;

public class User : Aggregate<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
    public string ProfileName { get; set; }
    public Media? ProfileImage { get; set; }
    public DateTime SignupDate { get; set; }

    public static User Create(
        Guid id,
        string email,
        string phoneNumber,
        string firstName,
        string lastName,
        string profileName
    )
    {
        return new User()
               {
                   Id = id,
                   Email = email,
                   PhoneNumber = phoneNumber,
                   FirstName = firstName,
                   LastName = lastName,
                   ProfileName = profileName
               };
    }

    public ICollection<Chat> Chats { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<Post> Posts { get; set; }
    public ICollection<Post> SavedPosts { get; set; }

    public ICollection<User> Followers { get; set; }

    // public ICollection<User> Followers => Subscriptions.Select(x => x.Follower).ToList();
    public ICollection<Subscription> FollowerSubscriptions { get; set; }

    public ICollection<Subscription> FollowingSubscriptions { get; set; }

    // public ICollection<User> Followings => Subscriptions.Select(x => x.Following).ToList();
    public ICollection<User> Followings { get; set; }
    public ICollection<Reaction> Reactions { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public NotificationSettings? NotificationSettings { get; set; }
    public PrivacySettings? PrivacySettings { get; set; }
}
