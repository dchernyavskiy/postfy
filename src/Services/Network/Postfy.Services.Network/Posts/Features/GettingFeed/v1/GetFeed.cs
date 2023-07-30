using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.GettingFeed.v1;

public record GetFeed() : ListQuery<GetFeedResponse>;

public class GetFeedHandler : IQueryHandler<GetFeed, GetFeedResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetFeedHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor, IMapper mapper)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetFeedResponse> Handle(GetFeed request, CancellationToken cancellationToken)
    {
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");

        var posts = await _context.Posts
                        .Where(p => p.User.Followers.Any(f => f.Id == userId))
                        .OrderBy(x => x.Created)
                        .ProjectTo<PostBriefDto>(_mapper.ConfigurationProvider)
                        .ApplyPagingAsync(request.Page, request.PageSize, cancellationToken);


        return new GetFeedResponse(posts);
    }
}
