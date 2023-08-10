using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Posts.Features.SavingPost.v1;

public class SavePostEndpoint : EndpointBaseAsync.WithRequest<SavePost>.WithActionResult<SavePostResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public SavePostEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPut(PostConfigs.PrefixUri + "/save", Name = "SavePost")]
    [ProducesResponseType(typeof(SavePostResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "SavePost",
        Description = "SavePost",
        OperationId = "SavePost",
        Tags = new[]
               {
                   PostConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<SavePostResponse>> HandleAsync(
        [FromBody] SavePost request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
