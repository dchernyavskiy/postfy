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

namespace Postfy.Services.Network.Posts.Features.GettingPosts.v1;

public record GetPosts(Guid? UserId) : ListQuery<GetPostsResponse>;

public class GetPostsHandler : IQueryHandler<GetPosts, GetPostsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetPostsHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor, IMapper mapper)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetPostsResponse> Handle(GetPosts request, CancellationToken cancellationToken)
    {
        var userId = request.UserId ?? _securityContextAccessor.GetIdAsGuid();
        var posts = await _context.Posts
                        .AsNoTracking()
                        .Include(x => x.Comments)
                        .Include(x => x.Reactions)
                        .Include(x => x.User)
                        .Where(x => x.UserId == userId)
                        .ProjectTo<PostBriefDto>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);

        return new GetPostsResponse(posts);
    }
}
