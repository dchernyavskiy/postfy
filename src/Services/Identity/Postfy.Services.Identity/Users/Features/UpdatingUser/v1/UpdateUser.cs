using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.Identity;
using Postfy.Services.Identity.Shared.Exceptions;
using Postfy.Services.Identity.Shared.Models;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;

namespace Postfy.Services.Identity.Users.Features.UpdatingUser.v1;

public record UpdateUser(Guid UserId, string FirstName, string LastName, string UserName) : ITxUpdateCommand;

public class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly IBus _bus;
    private readonly ILogger<UpdateUserHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserHandler(IBus bus, ILogger<UpdateUserHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _bus = bus;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.Against.NotFound(identityUser, new IdentityUserNotFoundException(request.UserId));

        if (string.IsNullOrEmpty(request.FirstName)) identityUser.FirstName = request.FirstName;
        if (string.IsNullOrEmpty(request.LastName)) identityUser.LastName = request.LastName;
        if (string.IsNullOrEmpty(request.UserName)) identityUser.UserName = request.UserName;

        await _userManager.UpdateAsync(identityUser);

        var userUpdated = new UserUpdatedV1(
            request.UserId,
            request.UserName,
            request.FirstName,
            request.LastName);

        await _bus.PublishAsync(userUpdated, null, cancellationToken);

        return Unit.Value;
    }
}
