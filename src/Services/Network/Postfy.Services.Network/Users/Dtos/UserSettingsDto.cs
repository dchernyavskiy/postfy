using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Dtos;

public class UserSettingsDto : IMapWith<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
    public MediaBriefDto? ProfileImage { get; set; }
    public NotificationSettings? NotificationSettings { get; set; }
    public PrivacySettings? PrivacySettings { get; set; }
}
