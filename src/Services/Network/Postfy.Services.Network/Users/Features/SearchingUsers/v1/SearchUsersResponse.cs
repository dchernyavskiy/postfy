using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.SearchingUsers.v1;

public record SearchUsersResponse(ICollection<UserBriefDto> Users);
