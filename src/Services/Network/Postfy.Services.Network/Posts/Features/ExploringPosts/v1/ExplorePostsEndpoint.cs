using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.ExploringPosts.v1;

public class ExplorePostsEndpoint : EndpointBaseAsync.WithRequest<ExplorePosts>.WithActionResult<ExplorePostsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public ExplorePostsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(PostConfigs.PrefixUri + "/explore", Name = "ExplorePosts")]
    [ProducesResponseType(typeof(ExplorePostsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "ExplorePosts",
        Description = "ExplorePosts",
        OperationId = "ExplorePosts",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<ExplorePostsResponse>> HandleAsync(
        [FromQuery] ExplorePosts request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
