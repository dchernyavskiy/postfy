using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Web.Extensions;
using Postfy.Services.Identity.Identity.Features.GettingClaims.v1;
using Postfy.Services.Identity.Identity.Features.Login.v1;
using Postfy.Services.Identity.Identity.Features.Logout.v1;
using Postfy.Services.Identity.Identity.Features.RefreshingToken.v1;
using Postfy.Services.Identity.Identity.Features.RevokingAccessToken.v1;
using Postfy.Services.Identity.Identity.Features.RevokingRefreshToken.v1;
using Postfy.Services.Identity.Identity.Features.SendingEmailVerificationCode.v1;
using Postfy.Services.Identity.Identity.Features.VerifyingEmail.v1;
using Postfy.Services.Identity.Shared.Extensions.WebApplicationBuilderExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Postfy.Services.Identity.Identity.Data;
using Postfy.Services.Identity.Shared;

namespace Postfy.Services.Identity.Identity;

internal class IdentityConfigs : IModuleConfiguration
{
    public const string Tag = "Identity";
    public const string IdentityPrefixUri = $"{SharedModulesConfiguration.IdentityModulePrefixUri}";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.AddCustomIdentity();

        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        if (builder.Environment.IsTest() == false)
            builder.AddCustomIdentityServer();

        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var identityVersionGroup = endpoints.MapApiGroup(Tag).WithTags(Tag);

        // create a new sub group for each version
        var identityGroupV1 = identityVersionGroup.MapGroup(IdentityPrefixUri).HasApiVersion(1.0);

        // create a new sub group for each version
        var identityGroupV2 = identityVersionGroup.MapGroup(IdentityPrefixUri).HasApiVersion(2.0);

        identityGroupV1
            .MapGet(
                "/user-role",
                [Authorize(
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = IdentityConstants.Role.User
                )]
                () => new { Role = IdentityConstants.Role.User }
            )
            .WithTags("Identity");

        identityGroupV1
            .MapGet(
                "/admin-role",
                [Authorize(
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = IdentityConstants.Role.Admin
                )]
                () => new { Role = IdentityConstants.Role.Admin }
            )
            .WithTags("Identity");



        identityGroupV1.MapLoginUserEndpoint();
        identityGroupV1.MapLogoutEndpoint();
        identityGroupV1.MapSendEmailVerificationCodeEndpoint();
        identityGroupV1.MapSendVerifyEmailEndpoint();
        // identityGroupV1.MapRefreshTokenEndpoint();
        identityGroupV1.MapRevokeTokenEndpoint();
        identityGroupV1.MapRevokeAccessTokenEndpoint();
        identityGroupV1.MapGetClaimsEndpoint();

        return endpoints;
    }
}
