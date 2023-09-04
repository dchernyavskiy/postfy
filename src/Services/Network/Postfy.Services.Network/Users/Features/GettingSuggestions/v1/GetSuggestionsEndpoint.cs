using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Users.Features.GettingSuggestions.v1;

public class GetSuggestionsEndpoint : EndpointBaseAsync.WithRequest<GetSuggestions>.WithActionResult<GetSuggestionsResponse>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetSuggestionsEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UserConfigs.PrefixUri + "/suggestions", Name = "GetSuggestions")]
    [ProducesResponseType(typeof(GetSuggestionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "GetSuggestions",
        Description = "GetSuggestions",
        OperationId = "GetSuggestions",
        Tags = new[]
               {
                   UserConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<GetSuggestionsResponse>> HandleAsync(
        [FromQuery] GetSuggestions request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        return await _queryProcessor.SendAsync(request, cancellationToken);
    }
}
