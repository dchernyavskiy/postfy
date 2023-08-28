using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.GettingPost.v1;

public class GetPostEndpoint : EndpointBaseAsync.WithRequest<GetPost>.WithActionResult<GetPostResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetPostEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(PostConfigs.PrefixUri + "/{PostId}", Name = "GetPost")]
    [ProducesResponseType(typeof(GetPostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetPost",
        Description = "GetPost",
        OperationId = "GetPost",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetPostResponse>> HandleAsync(
        [FromRoute]GetPost request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
