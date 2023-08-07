using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Network.Messages.Data;
using Postfy.Services.Network.Shared;

namespace Postfy.Services.Network.Messages;

public class MessageConfigs : IModuleConfiguration
{
    public const string Tag = "Messages";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/messages";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDataSeeder, MessageDataSeeder>();
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
