using Postfy.Services.Identity.Users.Dtos;
using Postfy.Services.Identity.Users.Dtos.v1;

namespace Postfy.Services.Identity.Users.Features.GettingUserById.v1;

internal record UserByIdResponse(IdentityUserDto IdentityUser);
