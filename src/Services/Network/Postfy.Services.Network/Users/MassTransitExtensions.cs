using Humanizer;
using MassTransit;
using Postfy.Services.Network.Shared.Extensions.StringExtensions;
using Postfy.Services.Network.Users.Features.RegisteringUser.v1.Events.Integration.External;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;
using RabbitMQ.Client;

namespace Postfy.Services.Network.Users;

internal static class MassTransitExtensions
{
    internal static void AddUserEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            nameof(UserRegisteredV1).Underscore().Prefixify(),
            re =>
            {
                re.ConfigureConsumeTopology = true;
                re.SetQuorumQueue();
                re.Bind(
                    $"{nameof(UserRegisteredV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = nameof(UserRegisteredV1).Underscore();
                        e.ExchangeType = ExchangeType.Fanout;
                    });
                re.ConfigureConsumer<UserRegisteredConsumer>(context);
                re.RethrowFaultedMessages();
            });
    }
}
