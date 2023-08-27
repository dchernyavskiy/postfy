using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Web.Extensions;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task SeedData(this WebApplication app)
    {
        if (!app.Environment.IsTest())
        {


            using var serviceScope = app.Services.CreateScope();
            var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>().OrderBy(x => x.Order);

            foreach (var seeder in seeders)
            {
                app.Logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                app.Logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
            }
        }
    }
}
