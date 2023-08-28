using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Types.Extensions;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Dtos;
using Postfy.Services.Network.Users.Models;
using SQLitePCL;

namespace Postfy.Services.Network.Posts.Features.GettingFeed.v1;

public record GetFeed() : ListQuery<GetFeedResponse>;

public class GetFeedHandler : IQueryHandler<GetFeed, GetFeedResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetFeedHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetFeedResponse> Handle(GetFeed request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var posts = await _context.Users
                        .Where(x => x.Id == userId)
                        .SelectMany(x => x.Followings.SelectMany(y => y.Posts))
                        .Include(x => x.User)
                        .Include(x => x.Reactions)
                        .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                        .OrderByDescending(x => x.Created)
                        .AsNoTracking()
                        .ProjectTo<PostBriefDto>(
                            _mapper.ConfigurationProvider,
                            new {currentUserId = userId})
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);

        return new GetFeedResponse(posts);
    }
}
