using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Posts.Features.GettingPosts.v1;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Posts.Features.GettingPost.v1;

public record GetPost(Guid PostId) : IQuery<GetPostResponse>;

public class GetPostHandler : IQueryHandler<GetPost, GetPostResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetPostHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor, IMapper mapper)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetPostResponse> Handle(GetPost request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
                       .Include(x => x.User)
                       .Include(x => x.Reactions)
                       .Include(x => x.Comments)
                       .ThenInclude(x => x.User)
                       .FirstOrDefaultAsync(
                           x => x.Id == request.PostId,
                           cancellationToken: cancellationToken);

        try
        {
            var userId = Guid.Parse(_securityContextAccessor.UserId);
            return new GetPostResponse(_mapper.Map<PostBriefDto>(post, opts => opts.Items.Add("UserId", userId)));
        }
        catch
        {
            // ignored
        }

        return new GetPostResponse(_mapper.Map<PostBriefDto>(post));
    }
}
