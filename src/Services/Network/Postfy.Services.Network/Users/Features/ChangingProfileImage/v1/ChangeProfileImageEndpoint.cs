using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.ChangingProfileImage.v1;

public class ChangeProfileImageEndpoint : EndpointBaseAsync.WithRequest<ChangeProfileImage>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public ChangeProfileImageEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(UserConfigs.PrefixUri + "/change-profile-image", Name = "ChangeProfileImage")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "ChangeProfileImage",
        Description = "ChangeProfileImage",
        OperationId = "ChangeProfileImage",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] ChangeProfileImage request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
