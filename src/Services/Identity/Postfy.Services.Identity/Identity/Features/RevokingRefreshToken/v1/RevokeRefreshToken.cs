using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Identity.Identity.Exceptions;
using Postfy.Services.Identity.Identity.Features.RefreshingToken.v1;
using Postfy.Services.Identity.Shared.Data;

namespace Postfy.Services.Identity.Identity.Features.RevokingRefreshToken.v1;

public record RevokeRefreshToken(string RefreshToken) : ICommand;

internal class RevokeRefreshTokenHandler : ICommandHandler<RevokeRefreshToken>
{
    private readonly IdentityContext _context;

    public RevokeRefreshTokenHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RevokeRefreshToken request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RevokeRefreshToken));

        var refreshToken = await _context
            .Set<global::Postfy.Services.Identity.Shared.Models.RefreshToken>()
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken: cancellationToken);

        if (refreshToken == null)
            throw new RefreshTokenNotFoundException(refreshToken);

        if (!refreshToken.IsRefreshTokenValid())
            throw new InvalidRefreshTokenException(refreshToken);

        // revoke token and save
        refreshToken.RevokedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
