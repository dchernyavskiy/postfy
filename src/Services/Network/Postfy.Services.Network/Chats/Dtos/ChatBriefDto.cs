using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Messages.Dtos;

namespace Postfy.Services.Network.Chats.Dtos;

public class ChatBriefDto : IMapWith<Chat>
{
    public MessageBriefDto LastMessage { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Chat, ChatBriefDto>()
            .ForMember(x => x.LastMessage, opts => opts.MapFrom(src => src.Messages.OrderBy(x => x.CreatedBy).Last()));
    }
}
