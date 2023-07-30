using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.GettingFeed.v1;

public class GetFeedEndpoint : EndpointBaseAsync.WithRequest<GetFeed>.WithActionResult<GetFeedResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetFeedEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(PostConfigs.PrefixUri + "/get-feed", Name = "GetFeed")]
    [ProducesResponseType(typeof(GetFeedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetFeed",
        Description = "GetFeed",
        OperationId = "GetFeed",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetFeedResponse>> HandleAsync(
        [FromBody] GetFeed request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
