using BuildingBlocks.Abstractions.CQRS.Commands;
using Elastic.Clients.Elasticsearch;
using MediatR.Pipeline;
using Nest;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;
using ILogger = Serilog.ILogger;

namespace Postfy.Services.Network.Users.Features.RegisteringUser.v1;

public record RegisterUser
(
    Guid Id,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName,
    string UserName
) : ICommand<RegisterUserResponse>;

public record RegisterUserResponse(User User);

public class RegisterUserHandler : ICommandHandler<RegisterUser, RegisterUserResponse>
{
    private readonly INetworkDbContext _context;

    public RegisterUserHandler(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var user = User.Create(
            request.Id,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.UserName);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterUserResponse(user);
    }
}

public class RegisterUserPostProcessor : IRequestPostProcessor<RegisterUser, RegisterUserResponse>
{
    private readonly IElasticClient _client;
    private readonly ILogger _logger;

    public RegisterUserPostProcessor(IElasticClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task Process(RegisterUser request, RegisterUserResponse response, CancellationToken cancellationToken)
    {
        try
        {
            var res = await _client.IndexAsync(response.User, i => i.Index(nameof(User).ToLower()), cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error in Process");
        }
    }
}
