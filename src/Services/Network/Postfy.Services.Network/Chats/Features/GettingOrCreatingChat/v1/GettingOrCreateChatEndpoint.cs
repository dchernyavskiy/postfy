using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Chats.Features.GettingOrCreatingChat.v1;

public class
    GetOrCreateChatEndpoint : EndpointBaseAsync.WithRequest<GetOrCreateChat>.WithActionResult<GetOrCreateChatResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public GetOrCreateChatEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(ChatConfigs.PrefixUri + "/get-or-create", Name = "GetOrCreateChat")]
    [ProducesResponseType(typeof(GetOrCreateChatResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetOrCreateChat",
        Description = "GetOrCreateChat",
        OperationId = "GetOrCreateChat",
        Tags = new[]
               {
                   ChatConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetOrCreateChatResponse>> HandleAsync(
        [FromBody] GetOrCreateChat request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
