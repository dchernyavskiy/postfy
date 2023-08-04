using BuildingBlocks.Core.Messaging;

namespace Postfy.Services.Shared.Identity.Users.Events.v1.Integration;

public record UserUpdatedV1(
    Guid IdentityId,
    string UserName,
    string FirstName,
    string LastName
) : IntegrationEvent;
