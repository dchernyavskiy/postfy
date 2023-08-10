using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Posts;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.FollowingUser.v1;

public class FollowUserEndpoint : EndpointBaseAsync.WithRequest<FollowUser>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public FollowUserEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(UserConfigs.PrefixUri, Name = "FollowUser")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "FollowUser",
        Description = "FollowUser",
        OperationId = "FollowUser",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] FollowUser request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
