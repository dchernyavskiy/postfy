using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingFollowings.v1;

public record GetFollowings() : ListQuery<GetFollowingsResponse>;

public class GetFollowingsHandler : IQueryHandler<GetFollowings, GetFollowingsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetFollowingsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetFollowingsResponse> Handle(GetFollowings request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var users = await _context.Users
                        .Where(u => u.Id == userId)
                        .SelectMany(x => x.Followings)
                        .ProjectTo<UserBriefDtoWithFollowerCount>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);

        return new GetFollowingsResponse(users);
    }
}
