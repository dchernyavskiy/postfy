using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Identity.Identity.Features.RefreshingToken.v1;

public class
    RefreshTokenEndpoint : EndpointBaseAsync.WithRequest<RefreshTokenRequest>.WithActionResult<RefreshTokenResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public RefreshTokenEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [AllowAnonymous]
    [HttpPost(IdentityConfigs.IdentityPrefixUri + "/refresh-token", Name = "RefreshToken")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "RefreshToken",
        Description = "RefreshToken",
        OperationId = "RefreshToken",
        Tags = new[]
               {
                   IdentityConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<RefreshTokenResponse>> HandleAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        using (var st = new StreamReader(Request.Body))
        {
            var s = await st.ReadToEndAsync();
        }

        var command = new RefreshToken(request.AccessToken, request.RefreshToken);

        var result = await _commandProcessor.SendAsync(command, cancellationToken);

        return result;
    }
}





























