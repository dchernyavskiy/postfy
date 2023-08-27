using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Chats.Features.GettingChat.v1;

public class GetChatEndpoint : EndpointBaseAsync.WithRequest<GetChat>.WithActionResult<GetChatResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public GetChatEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpGet(ChatConfigs.PrefixUri, Name = "GetChat")]
    [ProducesResponseType(typeof(GetChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetChat",
        Description = "GetChat",
        OperationId = "GetChat",
        Tags = new[]
               {
                   ChatConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetChatResponse>> HandleAsync(
        [FromQuery] GetChat request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
