using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Utils;
using BuildingBlocks.Security.Jwt;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Postfy.Services.Identity.Identity.Exceptions;
using Postfy.Services.Identity.Identity.Features.GeneratingJwtToken.v1;
using Postfy.Services.Identity.Identity.Features.GeneratingRefreshToken.v1;
using Postfy.Services.Identity.Shared.Data;
using Postfy.Services.Identity.Shared.Exceptions;
using Postfy.Services.Identity.Shared.Models;
using Postfy.Services.Identity.Users.Features.RegisteringUser.v1;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;

namespace Postfy.Services.Identity.Identity.Features.LoginWithGoogle.v1;

public record LoginWithGoogle(string Credential) : ICommand<LoginWithGoogleResponse>, ITxRequest;

internal class LoginWithGoogleValidator : AbstractValidator<LoginWithGoogle>
{
    public LoginWithGoogleValidator()
    {
        // RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("UserNameOrEmail cannot be empty");
        // RuleFor(x => x.Password).NotEmpty().WithMessage("password cannot be empty");
    }
}

internal class LoginWithGoogleHandler : ICommandHandler<LoginWithGoogle, LoginWithGoogleResponse>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IJwtService _jwtService;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<LoginWithGoogleHandler> _logger;
    private readonly IQueryProcessor _queryProcessor;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessagePersistenceService _messagePersistenceService;

    public LoginWithGoogleHandler(
        UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IJwtService jwtService,
        IOptions<JwtOptions> jwtOptions,
        SignInManager<ApplicationUser> signInManager,
        IdentityContext context,
        ILogger<LoginWithGoogleHandler> logger,
        IMessagePersistenceService messagePersistenceService
    )
    {
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _messagePersistenceService = messagePersistenceService;
    }

    private async Task<ApplicationUser> RegisterAsync(GoogleJsonWebSignature.Payload payload)
    {
        var applicationUser = new ApplicationUser
                              {
                                  FirstName = payload.GivenName,
                                  LastName = payload.FamilyName,
                                  UserName = Guid.NewGuid().ToString().Substring(0, 10),
                                  Email = payload.Email,
                                  UserState = UserState.Active,
                                  CreatedAt = DateTime.UtcNow,
                              };
        var identityResult = await _userManager.CreateAsync(applicationUser);
        if (!identityResult.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        var roles = new List<string> {IdentityConstants.Role.User};
        var roleResult = await _userManager.AddToRolesAsync(
                             applicationUser,
                             roles);

        if (!roleResult.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        var userRegistered = new UserRegisteredV1(
            applicationUser.Id,
            applicationUser.Email,
            applicationUser.PhoneNumber!,
            applicationUser.UserName,
            applicationUser.FirstName,
            applicationUser.LastName,
            roles);
        await _messagePersistenceService.AddPublishMessageAsync(
            new MessageEnvelope<UserRegisteredV1>(userRegistered, new Dictionary<string, object?>()));

        return applicationUser;
    }

    public async Task<LoginWithGoogleResponse> Handle(LoginWithGoogle request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(LoginWithGoogle));

        var settings = new GoogleJsonWebSignature.ValidationSettings() {Audience = new List<string>()};
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);
        var identityUser = await _userManager.FindByEmailAsync(payload.Email) ??
                           await RegisterAsync(payload);


        Guard.Against.Null(identityUser, new IdentityUserNotFoundException(request.Credential));

        await _signInManager.SignInAsync(identityUser, false);
        // instead of PasswordSignInAsync, we use CheckPasswordSignInAsync because we don't want set cookie, instead we use JWT
        // var signinResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, false);
        //
        // if (signinResult.IsNotAllowed)
        // {
        //     if (!await _userManager.IsEmailConfirmedAsync(identityUser))
        //         throw new EmailNotConfirmedException(identityUser.Email);
        //
        //     if (!await _userManager.IsPhoneNumberConfirmedAsync(identityUser))
        //         throw new PhoneNumberNotConfirmedException(identityUser.PhoneNumber);
        // }
        // else if (signinResult.IsLockedOut)
        // {
        //     throw new UserLockedException(identityUser.Id.ToString());
        // }
        // else if (signinResult.RequiresTwoFactor)
        // {
        //     throw new RequiresTwoFactorException("Require two factor authentication.");
        // }
        // else if (!signinResult.Succeeded)
        // {
        //     throw new PasswordIsInvalidException("Password is invalid.");
        // }

        var refreshToken = (
                               await _commandProcessor.SendAsync(
                                   new GenerateRefreshToken(identityUser.Id),
                                   cancellationToken)
                           ).RefreshToken;

        var accessToken = await _commandProcessor.SendAsync(
                              new GenerateJwtToken(identityUser, refreshToken.Token),
                              cancellationToken);

        if (string.IsNullOrWhiteSpace(accessToken.Token)) throw new AppException("Generate access token failed.");

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        if (_jwtOptions.CheckRevokedAccessTokens)
        {
            await _context
                .Set<AccessToken>()
                .AddAsync(
                    new AccessToken
                    {
                        UserId = identityUser.Id,
                        Token = accessToken.Token,
                        CreatedAt = DateTime.Now,
                        ExpiredAt = accessToken.ExpireAt,
                        CreatedByIp = IpUtilities.GetIpAddress()
                    },
                    cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
        return new LoginWithGoogleResponse(identityUser, accessToken.Token, refreshToken.Token);
    }
}
