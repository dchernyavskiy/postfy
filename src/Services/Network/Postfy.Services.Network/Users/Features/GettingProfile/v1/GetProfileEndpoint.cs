using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Posts;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.GettingProfile.v1;

public class GetProfileEndpoint : EndpointBaseAsync.WithRequest<GetProfile>.WithActionResult<GetProfileResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetProfileEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/profile", Name = "GetProfile")]
    [ProducesResponseType(typeof(GetProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetProfile",
        Description = "GetProfile",
        OperationId = "GetProfile",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetProfileResponse>> HandleAsync(
        [FromQuery] GetProfile request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
