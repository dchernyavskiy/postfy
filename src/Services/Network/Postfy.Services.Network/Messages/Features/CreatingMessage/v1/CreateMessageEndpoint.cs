using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

public class CreateMessageEndpoint : EndpointBaseAsync.WithRequest<CreateMessage>.WithActionResult<CreateMessageResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateMessageEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(MessageConfigs.PrefixUri, Name = "CreateMessage")]
    [ProducesResponseType(typeof(CreateMessageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "CreateMessage",
        Description = "CreateMessage",
        OperationId = "CreateMessage",
        Tags = new[]
               {
                   MessageConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<CreateMessageResponse>> HandleAsync(
        [FromBody] CreateMessage request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
