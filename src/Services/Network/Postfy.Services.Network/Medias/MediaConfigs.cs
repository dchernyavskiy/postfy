using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core.Web.Extenions.ServiceCollection;
using Postfy.Services.Network.Medias.Options;
using Postfy.Services.Network.Medias.Services.Contracts;
using Postfy.Services.Network.Shared;
using FileShare = Postfy.Services.Network.Medias.Services.FileShare;

namespace Postfy.Services.Network.Medias;

public class MediaConfigs : IModuleConfiguration
{
    public const string Tag = "Medias";
    public const string PrefixUri = $"{SharedModulesConfiguration.ModulePrefixUri}/medias";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IFileShare, FileShare>();
        builder.Services.AddConfigurationOptions<FileShareOptions>();
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
