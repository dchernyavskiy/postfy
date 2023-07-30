using Postfy.Services.Identity.Users.Dtos;
using Postfy.Services.Identity.Users.Dtos.v1;

namespace Postfy.Services.Identity.Users.Features.RegisteringUser.v1;

internal record RegisterUserResponse(IdentityUserDto? UserIdentity);
