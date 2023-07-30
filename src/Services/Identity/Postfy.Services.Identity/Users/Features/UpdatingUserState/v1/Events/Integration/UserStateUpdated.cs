using BuildingBlocks.Core.Messaging;
using Postfy.Services.Identity.Shared.Models;

namespace Postfy.Services.Identity.Users.Features.UpdatingUserState.v1.Events.Integration;

public record UserStateUpdated(Guid UserId, UserState OldUserState, UserState NewUserState) : IntegrationEvent;
