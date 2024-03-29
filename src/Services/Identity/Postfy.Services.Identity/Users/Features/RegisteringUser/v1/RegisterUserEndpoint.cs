using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Identity.Users.Features.RegisteringUser.v1;

public static class RegisterUserEndpoint
{
    internal static RouteHandlerBuilder MapRegisterNewUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", RegisterUser)
            .AllowAnonymous()
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RegisterUser")
            .WithDisplayName("Register New user.")
            .WithMetadata(new SwaggerOperationAttribute("Register New User.", "Register New User."))
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> RegisterUser(
        RegisterUserRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterUser(
            request.FirstName,
            request.LastName,
            request.UserName,
            request.Email,
            request.PhoneNumber,
            request.Password,
            request.ConfirmPassword,
            request.Roles?.ToList()
        );

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{UsersConfigs.UsersPrefixUri}/{result.UserIdentity?.Id}", result);
    }
}
