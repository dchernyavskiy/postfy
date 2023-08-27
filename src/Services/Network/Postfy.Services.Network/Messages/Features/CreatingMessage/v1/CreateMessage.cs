using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats;
using Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Features.CreatingMessage.v1;

public record CreateMessage
    (string Text, Guid ChatId, Guid? PostId, Guid? ParentId) : ICreateCommand<CreateMessageResponse>;

public class CreateMessageHandler : ICommandHandler<CreateMessage, CreateMessageResponse>
{
    private readonly INetworkDbContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IMapper _mapper;

    public CreateMessageHandler(
        INetworkDbContext context,
        ISecurityContextAccessor securityContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _securityContextAccessor = securityContextAccessor;
        _mapper = mapper;
    }

    public async Task<CreateMessageResponse> Handle(CreateMessage request, CancellationToken cancellationToken)
    {
        var userId = _securityContextAccessor.GetIdAsGuid();
        var message = new Message()
                      {
                          Text = request.Text,
                          PostId = request.PostId,
                          ChatId = request.ChatId,
                          ParentId = request.ParentId,
                          IsPost = request.PostId != null,
                          SenderId = userId
                      };

        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await _context.Messages
                      .Include(x => x.Sender)
                      .Include(x => x.Post)
                      .ProjectTo<MessageBriefDto>(_mapper.ConfigurationProvider, new {currentUserId = userId})
                      .FirstAsync(x => x.Id == message.Id, cancellationToken: cancellationToken);
        return new CreateMessageResponse(dto);
    }
}
