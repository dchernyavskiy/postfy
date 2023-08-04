using System.Threading.RateLimiting;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Caching.Behaviours;
using BuildingBlocks.Core.Mapping;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Registrations;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Email;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Integration.MassTransit;
using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.Persistence.EfCore.Postgres;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using BuildingBlocks.Swagger;
using BuildingBlocks.Validation;
using BuildingBlocks.Web.Extensions;
using Postfy.Services.Network.Users;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationBuilderExtensions;

internal static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddCore(builder.Configuration);

        builder.Services.AddCustomJwtAuthentication(builder.Configuration);
        builder.Services.AddCustomAuthorization(
            rolePolicies: new List<RolePolicy>
                          {
                              new(NetworkConstants.Role.Admin, new List<string> {NetworkConstants.Role.Admin}),
                              new(NetworkConstants.Role.User, new List<string> {NetworkConstants.Role.User})
                          });

        // https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps#environment-variables-and-configuration
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#non-prefixed-environment-variables
        builder.Configuration.AddEnvironmentVariables("postfy_network_env_");

        // https://github.com/tonerdo/dotnet-env
        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.AddCustomProblemDetails();

        builder.AddCustomSerilog();

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(typeof(NetworkAssemblyInfo).Assembly);

        builder.Services.AddHttpContextAccessor();

        if (builder.Environment.IsTest() == false)
        {
            builder.AddCustomHealthCheck(
                healthChecksBuilder =>
                {
                    var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>(nameof(PostgresOptions));
                    var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

                    Guard.Against.Null(postgresOptions, nameof(postgresOptions));
                    Guard.Against.Null(rabbitMqOptions, nameof(rabbitMqOptions));

                    healthChecksBuilder
                        .AddNpgSql(
                            postgresOptions.ConnectionString,
                            name: "OrdersService-Postgres-Check",
                            tags: new[]
                                  {
                                      "postgres",
                                      "database",
                                      "infra",
                                      "orders-service",
                                      "live",
                                      "ready"
                                  })
                        .AddRabbitMQ(
                            rabbitMqOptions.ConnectionString,
                            name: "OrdersService-RabbitMQ-Check",
                            timeout: TimeSpan.FromSeconds(3),
                            tags: new[]
                                  {
                                      "rabbitmq",
                                      "bus",
                                      "infra",
                                      "orders-service",
                                      "live",
                                      "ready"
                                  });
                });
        }

        builder.Services.AddEmailService(builder.Configuration);

        builder.Services.AddCqrs(
            pipelines: new[]
                       {
                           typeof(RequestValidationBehavior<,>),
                           typeof(StreamRequestValidationBehavior<,>),
                           typeof(StreamLoggingBehavior<,>),
                           typeof(StreamCachingBehavior<,>),
                           typeof(LoggingBehavior<,>),
                           typeof(CachingBehavior<,>),
                           typeof(InvalidateCachingBehavior<,>),
                           typeof(EfTxBehavior<,>)
                       });

        builder.Services.AddPostgresMessagePersistence(builder.Configuration);

        // https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
        builder.Services.AddRateLimiter(
            options =>
            {
                // rate limiter that limits all to 10 requests per minute, per authenticated username (or hostname if not authenticated)
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.Identity?.Name ??
                                          httpContext.Request.Headers.Host.ToString(),
                            factory: partition =>
                                         new FixedWindowRateLimiterOptions
                                         {
                                             AutoReplenishment = true,
                                             PermitLimit = 10,
                                             QueueLimit = 0,
                                             Window = TimeSpan.FromMinutes(1)
                                         }));
            });

        builder.AddCustomMassTransit(
            (context, cfg) =>
            {
                cfg.AddUserEndpoints(context);
            },
            autoConfigEndpoints: false);

        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());
        builder.AddCustomAutoMapper(Assembly.GetExecutingAssembly());

        builder.AddCustomCaching();

        return builder;
    }
}
