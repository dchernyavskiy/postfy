using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Posts;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.GettingSettings.v1;

public class GetSettingsEndpoint : EndpointBaseAsync.WithRequest<GetSettings>.WithActionResult<GetSettingsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetSettingsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/settings", Name = "GetSettings")]
    [ProducesResponseType(typeof(GetSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetSettings",
        Description = "GetSettings",
        OperationId = "GetSettings",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetSettingsResponse>> HandleAsync(
        [FromQuery] GetSettings request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
