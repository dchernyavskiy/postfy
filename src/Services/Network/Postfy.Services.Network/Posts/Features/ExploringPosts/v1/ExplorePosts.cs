using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.ExploringPosts.v1;

public record ExplorePosts(Guid? LastPostId) : ListQuery<ExplorePostsResponse>;

public class ExplorePostsHandler : IQueryHandler<ExplorePosts, ExplorePostsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public ExplorePostsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<ExplorePostsResponse> Handle(ExplorePosts request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var lastPostId = request.LastPostId ?? Guid.Empty;
        var posts = await _context.Posts
                        .Where(x => x.User.Followers.All(u => u.Id != userId))
                        .OrderByDescending(x => x.Created)
                        .ProjectTo<PostBriefDto>(_mapper.ConfigurationProvider)
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken: cancellationToken);
        var ss = await _context.Posts.FromSqlRaw(
                         @"select * from network.posts p
                order by p.created desc
                offset (
	                select rn
	                from (
		                select id, row_number() over(order by created) as rn from network.posts
	                ) T
	                 where T.id = {0}
                ) rows
	            fetch next {1} rows only
	",
                         lastPostId,
                         request.PageSize)
                     .ToListAsync(cancellationToken: cancellationToken);

        return new ExplorePostsResponse(
            posts,
            posts.Items.LastOrDefault()?.Id ?? Guid.Empty);
    }
}
