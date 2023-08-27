using Ardalis.GuardClauses;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Resiliency.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Resiliency.Extensions;

public static partial class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (sp, _) =>
            {
                var options = sp.GetRequiredService<IConfiguration>().BindOptions<PolicyOptions>(nameof(PolicyOptions));

                Guard.Against.Null(options, nameof(options));

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var retryLogger = loggerFactory.CreateLogger("PollyHttpRetryPoliciesLogger");

                return HttpRetryPolicies.GetHttpRetryPolicy(retryLogger, options);
            }
        );
    }
}
