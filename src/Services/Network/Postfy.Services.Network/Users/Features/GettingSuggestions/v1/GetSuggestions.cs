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

namespace Postfy.Services.Network.Users.Features.GettingSuggestions.v1;

public record GetSuggestions() : ListQuery<GetSuggestionsResponse>;

public class GetSuggestionsHandler : IQueryHandler<GetSuggestions, GetSuggestionsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetSuggestionsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetSuggestionsResponse> Handle(GetSuggestions request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var users = await _context.Users
                        .Where(u => u.Followers.All(x => x.Id != userId) && u.Followings.All(x => x.Id != userId))
                        .ProjectTo<UserBriefDtoWithFollowerCount>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);

        return new GetSuggestionsResponse(users);
    }
}
