using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Features.ChangingNotificationsSettings.v1;

public record ChangeNotificationsSettings
    (NotificationSettings Settings) : IUpdateCommand<ChangeNotificationsSettingsResponse>;

public class
    ChangeNotificationsSettingsHandler : ICommandHandler<ChangeNotificationsSettings,
        ChangeNotificationsSettingsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public ChangeNotificationsSettingsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<ChangeNotificationsSettingsResponse> Handle(
        ChangeNotificationsSettings request,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request.Settings);
        var userId = _securityContextAccessor.GetIdAsGuid();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);

        Guard.Against.Null(user);

        user.NotificationSettings = request.Settings;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new ChangeNotificationsSettingsResponse();
    }
}
