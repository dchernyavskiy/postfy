using MassTransit;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;
using RabbitMQ.Client;

namespace Postfy.Services.Network.Users.Features.RegisteringUser.v1.Events.Integration.External;

public class UserRegisteredConsumer : IConsumer<UserRegisteredV1>
{
    private readonly INetworkDbContext _context;

    public UserRegisteredConsumer(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<UserRegisteredV1> context)
    {
        var user = new User()
                   {
                       FirstName = context.Message.FirstName,
                       LastName = context.Message.LastName,
                       ProfileName = context.Message.UserName
                   };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
