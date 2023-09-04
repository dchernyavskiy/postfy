using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.GettingFollowers.v1;

public class GetFollowersEndpoint : EndpointBaseAsync.WithRequest<GetFollowers>.WithActionResult<GetFollowersResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetFollowersEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/followers", Name = "GetFollowers")]
    [ProducesResponseType(typeof(GetFollowersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetFollowers",
        Description = "GetFollowers",
        OperationId = "GetFollowers",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetFollowersResponse>> HandleAsync(
        [FromQuery] GetFollowers request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
