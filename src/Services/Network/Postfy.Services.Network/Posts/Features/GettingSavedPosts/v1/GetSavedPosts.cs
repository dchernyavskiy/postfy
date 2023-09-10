using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Abstractions.Mapping;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
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

namespace Postfy.Services.Network.Posts.Features.GettingSavedPosts.v1;

public record GetSavedPosts() : ListQuery<GetSavedPostsResponse>;

public class GetSavedPostsHandler : IQueryHandler<GetSavedPosts, GetSavedPostsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetSavedPostsHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetSavedPostsResponse> Handle(GetSavedPosts request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var posts = await _context.Users
                        .AsNoTracking()
                        .Where(x => x.Id == userId)
                        .SelectMany(x => x.SavedPosts)
                        .Include(x => x.Comments)
                        .Include(x => x.Reactions)
                        .Include(x => x.User)
                        .ProjectTo<PostBriefDto>(
                            _mapper.ConfigurationProvider,
                            new {currentUserId = userId})
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);

        return new GetSavedPostsResponse(posts);
    }
}
