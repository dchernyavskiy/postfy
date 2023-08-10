using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;

namespace Postfy.Services.Network.Users.Features.UpdatingUser.v1.Events.Integration.External;

public class UserUpdatedConsumer : IConsumer<UserUpdatedV1>
{
    private readonly INetworkDbContext _context;

    public UserUpdatedConsumer(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<UserUpdatedV1> context)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == context.Message.IdentityId);
        Guard.Against.Null(user);

        if (string.IsNullOrEmpty(context.Message.FirstName)) user.FirstName = context.Message.FirstName;
        if (string.IsNullOrEmpty(context.Message.LastName)) user.LastName = context.Message.LastName;
        if (string.IsNullOrEmpty(context.Message.UserName)) user.ProfileName = context.Message.UserName;
    }
}
