using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Dtos;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Posts;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Chats.Features.GettingChat.v1;

public record GetChat(Guid Id) : ICreateCommand<GetChatResponse>;

public class GetChatHandler : ICommandHandler<GetChat, GetChatResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetChatHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor, IMapper mapper)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetChatResponse> Handle(GetChat request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var chat = await _context.Chats
                       .Include(x => x.Users)
                       .Include(x => x.Messages)
                       .ThenInclude(x => x.Sender)
                       .Include(x => x.Messages)
                       .ThenInclude(x => x.Post)
                       .Include(x => x.Messages)
                       .ThenInclude(x => x.Parent)
                       .ProjectTo<ChatDto>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        return new GetChatResponse(chat);
    }
}
