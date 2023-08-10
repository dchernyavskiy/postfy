using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Posts;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Comments.Features.CreatingComment.v1;

public class CreateCommentEndpoint : EndpointBaseAsync.WithRequest<CreateComment>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateCommentEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(CommentConfigs.PrefixUri, Name = "CreateComment")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "CreateComment",
        Description = "CreateComment",
        OperationId = "CreateComment",
        Tags = new[]
               {
                   CommentConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] CreateComment request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
