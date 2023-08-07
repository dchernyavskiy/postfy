using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Posts;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;

public class GetCommentsByPostIdEndpoint : EndpointBaseAsync.WithRequest<GetCommentsByPostId>.WithActionResult<
    GetCommentsByPostIdResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetCommentsByPostIdEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(CommentConfigs.PrefixUri, Name = "GetCommentsByPostId")]
    [ProducesResponseType(typeof(GetCommentsByPostIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetCommentsByPostId",
        Description = "GetCommentsByPostId",
        OperationId = "GetCommentsByPostId",
        Tags = new[]
               {
                   CommentConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetCommentsByPostIdResponse>> HandleAsync(
        [FromQuery] GetCommentsByPostId request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
