using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Chats.Features.GettingChats.v1;

public class GetChatsEndpoint : EndpointBaseAsync.WithRequest<GetChats>.WithActionResult<GetChatsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetChatsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(ChatConfigs.PrefixUri + "/get-chats", Name = "GetChats")]
    [ProducesResponseType(typeof(GetChatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetChats",
        Description = "GetChats",
        OperationId = "GetChats",
        Tags = new[]
               {
                   ChatConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetChatsResponse>> HandleAsync(
        [FromQuery] GetChats request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
