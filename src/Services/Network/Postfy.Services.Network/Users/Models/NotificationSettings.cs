namespace Postfy.Services.Network.Users.Models;

public class NotificationSettings
{
    public bool SomeoneSendMessage { get; set; } = true;
    public bool SomeoneLikedPost { get; set; } = true;
    public bool SomeoneFollowed { get; set; } = true;
    public bool SomeoneSendFollowRequest { get; set; } = true;
    public bool SomeoneMentioned { get; set; } = true;
}
