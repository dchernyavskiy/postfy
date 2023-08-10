using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Chats.Features.CreatingChat.v1;

public class CreateChatEndpoint : EndpointBaseAsync.WithRequest<CreateChat>.WithActionResult<CreateChatResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateChatEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(ChatConfigs.PrefixUri, Name = "CreateChat")]
    [ProducesResponseType(typeof(CreateChatResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "CreateChat",
        Description = "CreateChat",
        OperationId = "CreateChat",
        Tags = new[]
               {
                   ChatConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<CreateChatResponse>> HandleAsync(
        [FromBody] CreateChat request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
