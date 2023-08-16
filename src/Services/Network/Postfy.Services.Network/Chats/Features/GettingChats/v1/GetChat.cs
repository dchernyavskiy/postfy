using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Dtos;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Chats.Features.GettingChats.v1;

public record GetChats() : IQuery<GetChatsResponse>;

public class GetChatsHandler : IQueryHandler<GetChats, GetChatsResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public GetChatsHandler(INetworkDbContext context, ISecurityContextAccessor securityContextAccessor, IMapper mapper)
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<GetChatsResponse> Handle(GetChats request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();

        var chats = await _context.Chats
                       .Where(x => x.Users.Any(u => u.Id == userId))
                       .ProjectTo<ChatBriefDto>(_mapper.ConfigurationProvider)
                       .ToListAsync(cancellationToken: cancellationToken);

        return new GetChatsResponse(chats);
    }
}
