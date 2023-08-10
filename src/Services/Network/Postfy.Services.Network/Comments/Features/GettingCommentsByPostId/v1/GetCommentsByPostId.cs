using BuildingBlocks.Abstractions.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;

public record GetCommentsByPostId(Guid PostId) : IQuery<GetCommentsByPostIdResponse>;

public class GetCommentsByPostIdHandler : IQueryHandler<GetCommentsByPostId, GetCommentsByPostIdResponse>
{
    private readonly INetworkDbContext _context;

    public GetCommentsByPostIdHandler(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task<GetCommentsByPostIdResponse> Handle(
        GetCommentsByPostId request,
        CancellationToken cancellationToken
    )
    {
        var post = await _context.Comments
                       .Where(x => x.PostId == request.PostId)
                       .ToListAsync(cancellationToken: cancellationToken);

        return new GetCommentsByPostIdResponse(post);
    }
}
