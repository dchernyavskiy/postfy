using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Web.Module;
using MediatR.Pipeline;
using Postfy.Services.Network.Shared;
using Postfy.Services.Network.Users.Data;
using Postfy.Services.Network.Users.Features.RegisteringUser.v1;

namespace Postfy.Services.Network.Users;

public class UserConfigs : IModuleConfiguration
{
    public const string Tag = "Users";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/users";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDataSeeder, UserDataSeeder>();
        builder.Services.AddScoped<IDataSeeder, FollowDataSeeder>();
        builder.Services
            .AddTransient<IRequestPostProcessor<RegisterUser, RegisterUserResponse>, RegisterUserPostProcessor>();
        return builder;
    }

    public async Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return app;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}
