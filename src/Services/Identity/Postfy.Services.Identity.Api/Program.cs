using Bogus;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Core.Web.Extenions.ServiceCollection;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using Postfy.Services.Identity;
using Postfy.Services.Identity.Api.Extensions.ApplicationBuilderExtensions;
using Postfy.Services.Identity.Api.Middlewares;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Identity Service").Centered().Color(Color.FromInt32(new Faker().Random.Int(1, 255))));



var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(
    (context, options) =>
    {
        // Handling Captive Dependency Problem




        options.ValidateScopes =
            context.HostingEnvironment.IsDevelopment()
            || context.HostingEnvironment.IsTest()
            || context.HostingEnvironment.IsStaging();

        // Issue with masstransit #85
        // options.ValidateOnBuild = true;
    }
);

builder.Services
    .AddControllers(
        options => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()))
    )
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        // options.SerializerSettings.Converters.Add(new StringEnumConverter()); // sending enum string to and from client instead of number
    })
    .AddControllersAsServices();


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSingleton<RevokeAccessTokenMiddleware>();

builder.Services.AddValidatedOptions<AppOptions>();


builder.AddMinimalEndpoints();

/*----------------- Module Services Setup ------------------*/
builder.AddModulesServices();

var app = builder.Build();

/*----------------- Module Middleware Setup ------------------*/
await app.ConfigureModules();







app.UseAppCors();

app.UseRevokeAccessTokenMiddleware();


app.MapControllers();

/*----------------- Module Routes Setup ------------------*/
app.MapModulesEndpoints();


app.MapMinimalEndpoints();

if (!app.Environment.IsProduction())
{
    // swagger middleware should register last to discover all endpoints and its versions correctly
    app.UseCustomSwagger();
}

await app.RunAsync();

public partial class Program { }
