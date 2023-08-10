using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Identity.Users.Features.UpdatingUser.v1;

public class UpdateUserEndpoint : EndpointBaseAsync.WithRequest<UpdateUser>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public UpdateUserEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(UsersConfigs.UsersPrefixUri, Name = "UpdateUser")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "UpdateUser",
        Description = "UpdateUser",
        OperationId = "UpdateUser",
        Tags = new[]
               {
                   UsersConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] UpdateUser request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
