using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingSettings.v1;

public record GetSettings() : IQuery<GetSettingsResponse>;

public class GetSettingsHandler : IQueryHandler<GetSettings, GetSettingsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetSettingsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetSettingsResponse> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var user = await _context.Users
                       .Include(x => x.Followings)
                       .Include(x => x.Followers)
                       .Include(x => x.Posts)
                       .FirstOrDefaultAsync(
                           x => x.Id == userId,
                           cancellationToken: cancellationToken);


        return new GetSettingsResponse(_mapper.Map<UserSettingsDto>(user));
    }
}
