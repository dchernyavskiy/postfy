using MassTransit;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;
using RabbitMQ.Client;

namespace Postfy.Services.Network.Users.Features.RegisteringUser.v1.Events.Integration.External;

public class UserRegisteredConsumer : IConsumer<UserRegisteredV1>
{
    private readonly ISender _sender;

    public UserRegisteredConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<UserRegisteredV1> context)
    {
        var command = new RegisterUser(
            context.Message.IdentityId,
            context.Message.Email,
            context.Message.PhoneNumber,
            context.Message.FirstName,
            context.Message.LastName,
            context.Message.UserName);
        await _sender.Send(command);
    }
}
