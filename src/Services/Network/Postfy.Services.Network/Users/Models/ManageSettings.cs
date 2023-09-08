namespace Postfy.Services.Network.Users.Models;

public class PrivacySettings
{
    public Permissions? WhoCanFollowMe { get; set; } = Permissions.Everyone;
    public Permissions? WhoCanMessageMe { get; set; } = Permissions.Everyone;
}

public enum Permissions
{
    Everyone,
    Followings,
    Nobody
}
