using System.Threading.RateLimiting;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Caching.Behaviours;
using BuildingBlocks.Core.Mapping;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Registrations;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Core.Web.Extenions.ServiceCollection;
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
using Elastic.Clients.Elasticsearch;
using Nest;
using Postfy.Services.Network.Shared.Options;
using Postfy.Services.Network.Users;

namespace Postfy.Services.Network.Shared.Extensions.WebApplicationBuilderExtensions;

internal static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddCore(builder.Configuration);

        builder.Services.AddCustomJwtAuthentication(builder.Configuration);
        builder.Services.AddCustomAuthorization(
            rolePolicies: new List<RolePolicy>
                          {
                              new(NetworkConstants.Role.Admin, new List<string> {NetworkConstants.Role.Admin}),
                              new(NetworkConstants.Role.User, new List<string> {NetworkConstants.Role.User})
                          });

        builder.Configuration.AddEnvironmentVariables("postfy_network_env_");

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

        builder.AddCustomMassTransit(
            (context, cfg) =>
            {
                cfg.AddUserEndpoints(context);
            },
            autoConfigEndpoints: false);

        builder.Services.AddScoped<ISecurityContextAccessor, SecurityContextAccessor>();
        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());
        builder.AddCustomAutoMapper(Assembly.GetExecutingAssembly());

        builder.AddCustomCaching();

        builder.Services.AddConfigurationOptions<ElasticsearchOptions>();
        builder.Services.AddSingleton<ElasticsearchClient>(
            sp =>
            {
                var options = sp.GetRequiredService<ElasticsearchOptions>();
                var settings = new ElasticsearchClientSettings(new Uri(options.Url))
                    .DefaultIndex("default-index");
                return new ElasticsearchClient(settings);
            });
        builder.Services.AddSingleton<IElasticClient>(
            sp =>
            {
                var options = sp.GetRequiredService<ElasticsearchOptions>();
                var settings = new ConnectionSettings(new Uri(options.Url))
                    .DefaultIndex("default_index");
                return new ElasticClient(settings);
            });

        return builder;
    }
}
