using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingProfile.v1;

public record GetProfile(Guid UserId) : IQuery<GetProfileResponse>;

public class GetProfileHandler : IQueryHandler<GetProfile, GetProfileResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetProfileHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetProfileResponse> Handle(GetProfile request, CancellationToken cancellationToken)
    {
        var userId = request.UserId == Guid.Empty ? _securityContextAccessor.GetIdAsGuid() : request.UserId;

        var user = await _context.Users
                       .Include(x => x.Posts)
                       .FirstOrDefaultAsync(
                           x => x.Id == userId,
                           cancellationToken: cancellationToken);


        return new GetProfileResponse(_mapper.Map<UserDto>(user));
    }
}
