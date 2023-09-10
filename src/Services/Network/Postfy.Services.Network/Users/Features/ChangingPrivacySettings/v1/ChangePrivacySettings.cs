using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Features.ChangingPrivacySettings.v1;

public record ChangePrivacySettings(PrivacySettings Settings) : IUpdateCommand<ChangePrivacySettingsResponse>;

public class ChangePrivacySettingsHandler : ICommandHandler<ChangePrivacySettings, ChangePrivacySettingsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public ChangePrivacySettingsHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<ChangePrivacySettingsResponse> Handle(
        ChangePrivacySettings request,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request.Settings);
        var userId = _securityContextAccessor.GetIdAsGuid();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);

        Guard.Against.Null(user);

        user.PrivacySettings = request.Settings;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new ChangePrivacySettingsResponse();
    }
}
