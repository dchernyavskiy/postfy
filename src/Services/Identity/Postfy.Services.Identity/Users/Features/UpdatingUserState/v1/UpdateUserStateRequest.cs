using Postfy.Services.Identity.Shared.Models;

namespace Postfy.Services.Identity.Users.Features.UpdatingUserState.v1;

public record UpdateUserStateRequest
{
    public UserState UserState { get; init; }
}
