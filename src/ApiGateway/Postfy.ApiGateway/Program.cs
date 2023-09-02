using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Logging;
using Postfy.ApiGateway.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;
using Yarp.ReverseProxy.Transforms;

Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Verbose)
    .Enrich.FromLogContext()
    .WriteTo.SpectreConsole(
        "{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}",
        LogEventLevel.Verbose)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// https://docs.duendesoftware.com/identityserver/v5/bff/apis/remote/
// https://microsoft.github.io/reverse-proxy/articles/index.html
// https://microsoft.github.io/reverse-proxy/articles/authn-authz.html
// https://microsoft.github.io/reverse-proxy/articles/transforms.html
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("yarp"))
    // .AddTransforms<AccessTokenTransformProvider>()
    .AddTransforms(
        transforms =>
        {
            // https://microsoft.github.io/reverse-proxy/articles/transforms.html
            transforms.AddRequestTransform(
                transform =>
                {
                    var requestId = Guid.NewGuid().ToString("N");
                    var correlationId = Guid.NewGuid().ToString("N");

                    transform.ProxyRequest.Headers.Add("X-Request-InternalCommandId", requestId);
                    transform.ProxyRequest.Headers.Add("X-Correlation-InternalCommandId", correlationId);

                    return ValueTask.CompletedTask;
                });
        });

const string corsName = "AllowFrontend";
var cors = builder.Configuration.BindOptions<CorsOptions>(nameof(CorsOptions));

builder.Services.AddCors(
    (opts) =>
    {
        opts.AddPolicy(
            corsName,
            build =>
            {
                build.WithOrigins(cors.Uris.ToArray())
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

var app = builder.Build();

// request logging just log in information level and above as default
app.UseSerilogRequestLogging(
    opts =>
    {
        opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
        opts.GetLevel = LogEnricher.GetLogLevel;
    });

app.UseCors(corsName);

app.MapGet(
    "/",
    async (HttpContext context) =>
    {
        await context.Response.WriteAsync($"Postfy Gateway");
    });

app.MapReverseProxy();

await app.RunAsync();
