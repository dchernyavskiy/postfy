using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Users.Features.ChangingBio.v1;

public record ChangeBio(string Bio) : IUpdateCommand<ChangeBioResponse>;

public class ChangeBioHandler : ICommandHandler<ChangeBio, ChangeBioResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public ChangeBioHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<ChangeBioResponse> Handle(ChangeBio request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Bio);
        var userId = _securityContextAccessor.GetIdAsGuid();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);

        Guard.Against.Null(user);

        user.Bio = request.Bio;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new ChangeBioResponse();
    }
}
