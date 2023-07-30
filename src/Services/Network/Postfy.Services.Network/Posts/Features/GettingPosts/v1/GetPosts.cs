using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.GettingPosts.v1;

public record GetPosts() : ListQuery<GetPostsResponse>;

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
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");

        var posts = await _context.Posts
                        .Include(x => x.Comments)
                        .Include(x => x.Reactions)
                        .ProjectTo<PostBriefDto>(_mapper.ConfigurationProvider)
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);


        return new GetPostsResponse(posts);
    }
}
