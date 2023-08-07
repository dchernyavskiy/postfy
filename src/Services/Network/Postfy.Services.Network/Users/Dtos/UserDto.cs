using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Dtos;

public class UserDto : IMapWith<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileName { get; set; }
    public Media? ProfileImage { get; set; }
}
