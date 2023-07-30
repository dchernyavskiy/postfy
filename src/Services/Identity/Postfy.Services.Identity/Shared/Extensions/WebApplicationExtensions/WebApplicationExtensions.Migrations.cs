using Microsoft.EntityFrameworkCore;
using Postfy.Services.Identity.Shared.Data;

namespace Postfy.Services.Identity.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task ApplyDatabaseMigrations(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("PostgresOptions:UseInMemory") == false)
        {
            using var serviceScope = app.Services.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<IdentityContext>();

            app.Logger.LogInformation("Updating database...");

            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("Updated database");
        }
    }
}
