using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Data;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task ApplyDatabaseMigrations(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            using var serviceScope = app.Services.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<NetworkDbContext>();

            app.Logger.LogInformation("Updating catalog database...");

            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("Updated catalog database");
        }
    }
}
