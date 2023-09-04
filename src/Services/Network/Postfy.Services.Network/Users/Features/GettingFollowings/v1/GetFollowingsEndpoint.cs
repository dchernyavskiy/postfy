using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.GettingFollowings.v1;

public class GetFollowingsEndpoint : EndpointBaseAsync.WithRequest<GetFollowings>.WithActionResult<GetFollowingsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetFollowingsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/followings", Name = "GetFollowings")]
    [ProducesResponseType(typeof(GetFollowingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetFollowings",
        Description = "GetFollowings",
        OperationId = "GetFollowings",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetFollowingsResponse>> HandleAsync(
        [FromQuery] GetFollowings request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
