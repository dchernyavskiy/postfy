using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Mapping;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Dtos;

namespace Postfy.Services.Network.Posts.Features.UpdatingPost.v1;

public record UpdatePost(Guid Id, string Caption, ICollection<MediaBriefDto> Medias) : IUpdateCommand, IMapWith<Post>;

public class UpdatePostHandler : ICommandHandler<UpdatePost>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public UpdatePostHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdatePost request, CancellationToken cancellationToken)
    {
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");

        var post = await _context.Posts.FirstOrDefaultAsync(
                       x => x.Id == request.Id,
                       cancellationToken: cancellationToken);
        Guard.Against.Null(post);

        post = _mapper.Map<Post>(request);
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
