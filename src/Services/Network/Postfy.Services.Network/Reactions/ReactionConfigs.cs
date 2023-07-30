using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Network.Shared;

namespace Postfy.Services.Network.Reactions;

public class ReactionConfigs : IModuleConfiguration
{
    public const string Tag = "Reactions";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/reactions";

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
