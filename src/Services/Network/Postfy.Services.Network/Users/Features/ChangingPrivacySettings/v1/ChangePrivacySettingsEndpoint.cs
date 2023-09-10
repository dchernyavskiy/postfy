using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.ChangingPrivacySettings.v1;

public class ChangePrivacySettingsEndpoint : EndpointBaseAsync.WithRequest<ChangePrivacySettings>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public ChangePrivacySettingsEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(UserConfigs.PrefixUri + "/change-privacy-settings", Name = "ChangePrivacySettings")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "ChangePrivacySettings",
        Description = "ChangePrivacySettings",
        OperationId = "ChangePrivacySettings",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] ChangePrivacySettings request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
