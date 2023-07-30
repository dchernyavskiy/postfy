using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Messages.Features.DeletingMessage.v1;

public class DeleteMessageEndpoint : EndpointBaseAsync.WithRequest<DeleteMessage>.WithoutResult
{
    private readonly ICommandProcessor _commandProcessor;

    public DeleteMessageEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpDelete(MessageConfigs.PrefixUri, Name = "DeleteMessage")]
    [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "DeleteMessage",
        Description = "DeleteMessage",
        OperationId = "DeleteMessage",
        Tags = new[]
               {
                   MessageConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task HandleAsync(
        [FromBody] DeleteMessage request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
