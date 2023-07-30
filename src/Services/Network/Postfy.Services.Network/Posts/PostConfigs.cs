using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Network.Shared;

namespace Postfy.Services.Network.Posts;

public class PostConfigs : IModuleConfiguration
{
    public const string Tag = "Posts";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/posts";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
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
