using Postfy.Services.Identity.Shared.Models;

namespace Postfy.Services.Identity.Identity.Features.Login.v1;

public record LoginResponse
{
    public LoginResponse(ApplicationUser user, string accessToken, string refreshToken)
    {
        UserId = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.UserName;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public Guid UserId { get; }
    public string AccessToken { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Username { get; }
    public string RefreshToken { get; }
}
