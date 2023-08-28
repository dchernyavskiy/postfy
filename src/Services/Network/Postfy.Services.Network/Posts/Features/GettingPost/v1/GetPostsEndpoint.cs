using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.GettingPosts.v1;

public class GetPostsEndpoint : EndpointBaseAsync.WithRequest<GetPosts>.WithActionResult<GetPostsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetPostsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(PostConfigs.PrefixUri, Name = "GetPosts")]
    [ProducesResponseType(typeof(GetPostsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetPosts",
        Description = "GetPosts",
        OperationId = "GetPosts",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetPostsResponse>> HandleAsync(
        [FromQuery] GetPosts request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
