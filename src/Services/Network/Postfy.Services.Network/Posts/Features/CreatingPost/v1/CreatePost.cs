using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Jwt;
using Microsoft.FSharp.Core;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Posts.Features.CreatingPost.v1;

public record CreatePost(string Caption, ICollection<MediaBriefDto> Medias) : ICreateCommand<CreatePostResponse>;

public class CreatePostHandler : ICommandHandler<CreatePost, CreatePostResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public CreatePostHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<CreatePostResponse> Handle(CreatePost request, CancellationToken cancellationToken)
    {
        var userIdStr = _securityContextAccessor.UserId;
        Guard.Against.NullOrEmpty(userIdStr, "You are not authenticated.");
        var userId = Guid.Parse(userIdStr!);
        Guard.Against.NullOrEmpty(userId, "User id can't be empty.");

        var post = new Post()
                   {
                       Caption = request.Caption,
                       Medias = _mapper.ProjectTo<Media>(request.Medias.AsQueryable()).ToList(),
                       UserId = userId,
                   };

        await _context.Posts.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreatePostResponse(post);
    }
}
