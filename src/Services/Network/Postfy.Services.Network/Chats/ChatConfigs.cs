using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Network.Chats.Data;
using Postfy.Services.Network.Shared;

namespace Postfy.Services.Network.Chats;

public class ChatConfigs : IModuleConfiguration
{
    public const string Tag = "Chats";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/chats";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDataSeeder, ChatDataSeeder>();
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
