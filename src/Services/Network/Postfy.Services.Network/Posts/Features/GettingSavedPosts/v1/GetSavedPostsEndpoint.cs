using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.GettingSavedPosts.v1;

public class
    GetSavedPostsEndpoint : EndpointBaseAsync.WithRequest<GetSavedPosts>.WithActionResult<GetSavedPostsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetSavedPostsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(PostConfigs.PrefixUri + "/get-saved-posts", Name = "GetSavedPosts")]
    [ProducesResponseType(typeof(GetSavedPostsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetSavedPosts",
        Description = "GetSavedPosts",
        OperationId = "GetSavedPosts",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetSavedPostsResponse>> HandleAsync(
        [FromQuery] GetSavedPosts request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
