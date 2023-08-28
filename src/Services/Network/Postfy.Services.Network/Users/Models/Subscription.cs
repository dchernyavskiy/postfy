namespace Postfy.Services.Network.Users.Models;

public class Subscription
{
    public User Follower { get; set; }
    public Guid FollowerId { get; set; }
    public User Following { get; set; }
    public Guid FollowingId { get; set; }
}
