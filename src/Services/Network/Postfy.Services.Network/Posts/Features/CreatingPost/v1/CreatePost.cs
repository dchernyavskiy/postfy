using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.FSharp.Core;
using Postfy.Services.Network.Medias.Features.UploadingMedia.v1;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Posts.Features.CreatingPost.v1;

public record CreatePost(string Caption, IFormFileCollection Files) : ICreateCommand<CreatePostResponse>;

public class CreatePostHandler : ICommandHandler<CreatePost, CreatePostResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public CreatePostHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper,
        ISender sender
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<CreatePostResponse> Handle(CreatePost request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var medias = await _sender.Send(new UploadMedia(request.Files), cancellationToken);
        var post = new Post() {Caption = request.Caption, Medias = medias.Medias, UserId = userId,};

        await _context.Posts.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreatePostResponse(post);
    }
}
