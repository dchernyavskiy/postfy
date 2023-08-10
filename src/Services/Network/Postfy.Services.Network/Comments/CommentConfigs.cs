using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Web.Module;
using Postfy.Services.Network.Comments.Data;
using Postfy.Services.Network.Shared;

namespace Postfy.Services.Network.Comments;

public class CommentConfigs : IModuleConfiguration
{
    public const string Tag = "Comments";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/comments";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDataSeeder, CommentDataSeeder>();
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
