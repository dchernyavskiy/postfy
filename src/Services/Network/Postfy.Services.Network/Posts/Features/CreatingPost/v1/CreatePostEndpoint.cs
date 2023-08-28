using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.CreatingPost.v1;

public class CreatePostEndpoint : EndpointBaseAsync.WithRequest<CreatePost>.WithActionResult<CreatePostResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreatePostEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(PostConfigs.PrefixUri, Name = "CreatePost")]
    [ProducesResponseType(typeof(CreatePostResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "CreatePost",
        Description = "CreatePost",
        OperationId = "CreatePost",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<CreatePostResponse>> HandleAsync(
        [FromForm] CreatePost request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
