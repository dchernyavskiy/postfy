using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Users.Features.UpdatingUser.v1;

public record UpdateUser(Media ProfileImage) : IUpdateCommand<UpdateUserResponse>;

public class UpdateUserHandler : ICommandHandler<UpdateUser, UpdateUserResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateUserHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ProfileImage.Url);
        var userId = _securityContextAccessor.GetIdAsGuid();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);

        Guard.Against.Null(user);

        user.ProfileImage = request.ProfileImage;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateUserResponse();
    }
}
