using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.ChangingNotificationsSettings.v1;

public class
    ChangeNotificationsSettingsEndpoint : EndpointBaseAsync.WithRequest<ChangeNotificationsSettings>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public ChangeNotificationsSettingsEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(UserConfigs.PrefixUri + "/change-notification-settings", Name = "ChangeNotificationsSettings")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "ChangeNotificationsSettings",
        Description = "ChangeNotificationsSettings",
        OperationId = "ChangeNotificationsSettings",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] ChangeNotificationsSettings request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
