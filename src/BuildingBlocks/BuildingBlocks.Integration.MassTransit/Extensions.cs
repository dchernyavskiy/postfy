using System.Reflection;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Utils;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Validation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using IBus = BuildingBlocks.Abstractions.Messaging.IBus;

namespace BuildingBlocks.Integration.MassTransit;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomMassTransit(
        this WebApplicationBuilder builder,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureReceiveEndpoints = null,
        Action<IBusRegistrationConfigurator>? configureBusRegistration = null,
        bool autoConfigEndpoints = false,
        params Assembly[] scanAssemblies
    )
    {
        // - should be placed out of action delegate for getting correct calling assembly
        // - Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable (it is possible to get ReflectionTypeLoadException, because some dependent type assembly are lazy and not loaded yet), so we use `GetAllReferencedAssemblies` and it
        // load all referenced assemblies explicitly.
        var assemblies = scanAssemblies.Any()
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).ToArray();

        if (!builder.Environment.IsTest())
        {
            builder.Services.AddMassTransit(ConfiguratorAction);
        }
        else
        {
            builder.Services.AddMassTransitTestHarness(ConfiguratorAction);
        }

        void ConfiguratorAction(IBusRegistrationConfigurator busRegistrationConfigurator)
        {
            configureBusRegistration?.Invoke(busRegistrationConfigurator);


            busRegistrationConfigurator.AddConsumers(assemblies);

            // exclude namespace for the messages
            busRegistrationConfigurator.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));

            // Ref: https://masstransit-project.com/advanced/topology/rabbitmq.html








            busRegistrationConfigurator.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.FlattenHierarchy;

                    if (autoConfigEndpoints)
                    {

                        cfg.ConfigureEndpoints(context);
                    }

                    var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>();

                    cfg.Host(
                        rabbitMqOptions.Host,
                        rabbitMqOptions.Port,
                        "/",
                        hostConfigurator =>
                        {
                            hostConfigurator.Username(rabbitMqOptions.UserName);
                            hostConfigurator.Password(rabbitMqOptions.Password);
                        }
                    );



                    cfg.UseMessageRetry(r => AddRetryConfiguration(r));

                    // cfg.UseInMemoryOutbox();


                    cfg.Publish<IIntegrationEvent>(p => p.Exclude = true);
                    cfg.Publish<IntegrationEvent>(p => p.Exclude = true);
                    cfg.Publish<IMessage>(p => p.Exclude = true);
                    cfg.Publish<Message>(p => p.Exclude = true);
                    cfg.Publish<ITxRequest>(p => p.Exclude = true);

                    // for setting exchange name for message type as default. masstransit by default uses fully message type name for exchange name.
                    cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());

                    configureReceiveEndpoints?.Invoke(context, cfg);
                }
            );
        }

        builder.Services.AddTransient<IBus, MassTransitBus>();

        return builder;
    }

    private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator
            .Exponential(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(120), TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit

        return retryConfigurator;
    }
}
