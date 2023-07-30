using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Persistence.EfCore.Postgres;
using BuildingBlocks.Persistence.Mongo;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Data;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationBuilderExtensions;

internal static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddStorage(this WebApplicationBuilder builder)
    {
        AddPostgresWriteStorage(builder.Services, builder.Configuration);

        return builder;
    }

    private static void AddPostgresWriteStorage(IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("PostgresOptions.UseInMemory"))
        {
            services.AddDbContext<NetworkDbContext>(options => options.UseInMemoryDatabase("Postfy.Services.Customers"));

            services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<NetworkDbContext>()!);
        }
        else
        {
            services.AddPostgresDbContext<NetworkDbContext>();
        }

        services.AddScoped<INetworkDbContext>(provider => provider.GetRequiredService<NetworkDbContext>());
    }
}
