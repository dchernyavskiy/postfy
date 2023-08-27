using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Security.Extensions;

public static class Extensions
{
    public static AuthenticationBuilder AddCustomJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtOptions>? optionConfigurator = null
    )
    {




        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        AddJwtServices(services, configuration, optionConfigurator);

        var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
        Guard.Against.Null(jwtOptions, nameof(jwtOptions));




        // since .NET 7, the default scheme is no longer required, when we define just one authentication scheme and It is automatically inferred
        return services
            .AddAuthentication() // no default scheme specified
            .AddJwtBearer(options =>
            {
                //-- JwtBearerDefaults.AuthenticationScheme --
                options.Audience = jwtOptions.Audience;
                options.SaveToken = true;
                options.RefreshOnIssuerKeyNotFound = false;
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    SaveSigninToken = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new UnAuthorizedException("The Token is expired.");
                        }

                        throw new IdentityException(
                            context.Exception.Message,
                            statusCode: HttpStatusCode.InternalServerError
                        );
                    },
                    OnChallenge = context =>
                    {
                        // context.HandleResponse();
                        // if (!context.Response.HasStarted)
                        // {
                        //     throw new IdentityException(
                        //         "You are not Authorized.",
                        //         statusCode: HttpStatusCode.Unauthorized);
                        // }

                        return Task.CompletedTask;
                    },
                    OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource.")
                };
            });
    }

    public static IServiceCollection AddJwtServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtOptions>? optionConfigurator = null
    )
    {
        var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
        Guard.Against.Null(jwtOptions, nameof(jwtOptions));

        optionConfigurator?.Invoke(jwtOptions);

        if (optionConfigurator is { })
        {
            services.Configure(nameof(JwtOptions), optionConfigurator);
        }
        else
        {
            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(nameof(JwtOptions)))
                .ValidateDataAnnotations();
        }

        services.AddTransient<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddCustomAuthorization(
        this IServiceCollection services,
        IList<ClaimPolicy>? claimPolicies = null,
        IList<RolePolicy>? rolePolicies = null
    )
    {
        services.AddAuthorization(authorizationOptions =>
        {


            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme
            );
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            authorizationOptions.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();


            if (claimPolicies is { })
            {
                foreach (var policy in claimPolicies)
                {
                    authorizationOptions.AddPolicy(
                        policy.Name,
                        x =>
                        {
                            x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            foreach (var policyClaim in policy.Claims)
                            {
                                x.RequireClaim(policyClaim.Type, policyClaim.Value);
                            }
                        }
                    );
                }
            }


            if (rolePolicies is { })
            {
                foreach (var rolePolicy in rolePolicies)
                {
                    authorizationOptions.AddPolicy(
                        rolePolicy.Name,
                        x =>
                        {
                            x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                            x.RequireRole(rolePolicy.Roles);
                        }
                    );
                }
            }
        });

        return services;
    }

    public static void AddExternalLogins(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
        Guard.Against.Null(jwtOptions, nameof(jwtOptions));

        if (jwtOptions.GoogleLoginConfigs is { })
        {
            services
                .AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = jwtOptions.GoogleLoginConfigs.ClientId;
                    googleOptions.ClientSecret = jwtOptions.GoogleLoginConfigs.ClientId;
                    googleOptions.SaveTokens = true;
                });
        }
    }
}
