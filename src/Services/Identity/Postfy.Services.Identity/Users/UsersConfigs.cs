using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Identity.Users.Features.GettingUerByEmail.v1;
using Postfy.Services.Identity.Users.Features.GettingUserById.v1;
using Postfy.Services.Identity.Users.Features.RegisteringUser.v1;
using Postfy.Services.Identity.Users.Features.UpdatingUserState.v1;
using Postfy.Services.Identity.Shared;

namespace Postfy.Services.Identity.Users;

internal class UsersConfigs : IModuleConfiguration
{
    public const string Tag = "Users";
    public const string UsersPrefixUri = $"{SharedModulesConfiguration.IdentityModulePrefixUri}/users";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var usersVersionGroup = endpoints.MapApiGroup(Tag).WithTags(Tag);

        // create a new sub group for each version
        var usersGroupV1 = usersVersionGroup.MapGroup(UsersPrefixUri).HasApiVersion(1.0);

        // create a new sub group for each version
        var usersGroupV2 = usersVersionGroup.MapGroup(UsersPrefixUri).HasApiVersion(2.0);



        usersGroupV1.MapRegisterNewUserEndpoint();
        usersGroupV1.MapUpdateUserStateEndpoint();
        usersGroupV1.MapGetUserByIdEndpoint();
        usersGroupV1.MapGetUserByEmailEndpoint();

        return endpoints;
    }
}
