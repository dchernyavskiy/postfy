using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.SearchingUsers.v1;

public class SearchUsersEndpoint : EndpointBaseAsync.WithRequest<SearchUsers>.WithActionResult<SearchUsersResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public SearchUsersEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/search", Name = "SearchUsers")]
    [ProducesResponseType(typeof(SearchUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "SearchUsers",
        Description = "SearchUsers",
        OperationId = "SearchUsers",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<SearchUsersResponse>> HandleAsync(
        [FromQuery] SearchUsers request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
