using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Grafana.Loki;

namespace BuildingBlocks.Logging;

public static class RegistrationExtensions
{
    public static WebApplicationBuilder AddCustomSerilog(
        this WebApplicationBuilder builder,
        string sectionName = "Serilog",
        Action<LoggerConfiguration>? extraConfigure = null
    )
    {
        var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>(sectionName);

        builder.Host.UseSerilog(
            (context, serviceProvider, loggerConfiguration) =>
            {
                extraConfigure?.Invoke(loggerConfiguration);

                loggerConfiguration.Enrich
                    .WithProperty("Application", builder.Environment.ApplicationName)
                    // .Enrich.WithSpan()
                    // .Enrich.WithBaggage()
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.FromLogContext()

                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()

                    .Enrich.WithExceptionDetails(
                        new DestructuringOptionsBuilder()
                            .WithDefaultDestructurers()
                            .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
                    );


                loggerConfiguration.ReadFrom.Configuration(context.Configuration, sectionName: sectionName);

                if (serilogOptions.UseConsole)
                {
                    if (serilogOptions.UseElasticsearchJsonFormatter)
                    {


                        loggerConfiguration.WriteTo.Async(
                            writeTo => writeTo.Console(new ExceptionAsObjectJsonFormatter(renderMessage: true))
                        );
                    }
                    else
                    {

                        loggerConfiguration.WriteTo.Async(
                            writeTo => writeTo.Console(outputTemplate: serilogOptions.LogTemplate)
                        );
                    }
                }


                if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
                {
                    // elasticsearch sink internally is async

                    loggerConfiguration.WriteTo.Elasticsearch(
                        new(new Uri(serilogOptions.ElasticSearchUrl))
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                            CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                            IndexFormat =
                                $"{
                                    builder.Environment.ApplicationName
                                }-{
                                    builder.Environment.EnvironmentName
                                }-{
                                    DateTime.Now
                                    :yyyy-MM}"
                        }
                    );
                }


                if (!string.IsNullOrEmpty(serilogOptions.GrafanaLokiUrl))
                {
                    loggerConfiguration.WriteTo.GrafanaLoki(
                        serilogOptions.GrafanaLokiUrl,
                        new[]
                        {
                            new LokiLabel { Key = "service", Value = "Postfy" }
                        },
                        new[] { "app" }
                    );
                }

                if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
                {
                    // seq sink internally is async
                    loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
                }


                if (serilogOptions.ExportLogsToOpenTelemetry)
                {
                    // export logs from serilog to opentelemetry
                    loggerConfiguration.WriteTo.OpenTelemetry();
                }

                if (!string.IsNullOrEmpty(serilogOptions.LogPath))
                {
                    loggerConfiguration.WriteTo.Async(
                        writeTo =>
                            writeTo.File(
                                serilogOptions.LogPath,
                                outputTemplate: serilogOptions.LogTemplate,
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true
                            )
                    );
                }
            }
        );

        return builder;
    }
}
