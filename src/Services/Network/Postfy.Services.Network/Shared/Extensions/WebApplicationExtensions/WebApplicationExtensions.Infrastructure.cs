using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.Web.Extensions;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Hubs;
using Serilog;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task UseInfrastructure(this WebApplication app)
    {
        // this middleware should be first middleware
        // request logging just log in information level and above as default
        app.UseSerilogRequestLogging(
            opts =>
            {
                opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;

                // this level wil use for request logging

                opts.GetLevel = LogEnricher.GetLogLevel;
            });

        // orders for middlewares is important and problemDetails middleware should be placed on top
        app.UseProblemDetails();

        app.UseRequestLogContextMiddleware();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ChatHub>("/signalr/v1/network/chat");

        await app.UsePostgresPersistenceMessage(app.Logger);
        app.UseCustomRateLimit();

        if (app.Environment.IsTest() == false) app.UseCustomHealthCheck();
    }
}
