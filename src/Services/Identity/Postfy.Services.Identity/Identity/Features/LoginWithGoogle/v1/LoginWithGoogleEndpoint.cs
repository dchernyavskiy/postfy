using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;

namespace Postfy.Services.Identity.Identity.Features.LoginWithGoogle.v1;

public static class LoginWithGoogleEndpoint
{
    internal static RouteHandlerBuilder MapLoginWithGoogleUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // https://github.com/dotnet/aspnetcore/issues/45082
        // https://github.com/dotnet/aspnetcore/issues/40753
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/2414
        return endpoints
            .MapPost("/login-with-google", LoginUserWithGoogle)
            .AllowAnonymous()
            .Produces<LoginWithGoogleResponse>()
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status500InternalServerError)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new(operation) {Summary = "Login User With Google", Description = "Login User With Google"})
            .WithDisplayName("Login User With Google.")
            .WithName("LoginWithGoogle")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> LoginUserWithGoogle(
        LoginWithGoogleRequest withGoogleRequest,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginWithGoogle(withGoogleRequest.Credential);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Ok(result);
    }
}
