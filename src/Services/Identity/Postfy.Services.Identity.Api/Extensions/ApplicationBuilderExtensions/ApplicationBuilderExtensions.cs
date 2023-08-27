namespace Postfy.Services.Identity.Api.Extensions.ApplicationBuilderExtensions;

public static class ApplicationBuilderExtensions
{

    ///     Register CORS.
    /// </summary>
    public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
    {
        app.UseCors(
            p =>
            {
                p.AllowAnyOrigin();
                p.AllowAnyMethod();
                p.AllowAnyHeader();
            });

        return app;
    }
}
