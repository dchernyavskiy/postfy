using Postfy.Services.Identity.Users.Dtos;
using Postfy.Services.Identity.Users.Dtos.v1;

namespace Postfy.Services.Identity.Users.Features.GettingUerByEmail.v1;

public record GetUserByEmailResponse(IdentityUserDto? UserIdentity);
