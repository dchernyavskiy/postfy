using Postfy.Services.Identity.Identity.Dtos;
using Postfy.Services.Identity.Identity.Dtos.v1;

namespace Postfy.Services.Identity.Identity.Features.GeneratingRefreshToken.v1;

public record GenerateRefreshTokenResponse(RefreshTokenDto RefreshToken);
