using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Newtonsoft.Json;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Users.Dtos;
using Riok.Mapperly.Abstractions;

namespace Postfy.Services.Network.Chats.Dtos;

public class ChatBriefDto : IMapWith<Chat>
{
    public Guid Id { get; set; }
    public UserBriefDto? Guest => Guests.FirstOrDefault();

    [JsonIgnore]
    public ICollection<UserBriefDto> Guests { get; set; }

    public MessageBriefDto LastMessage => LastMessages.LastOrDefault(new MessageBriefDto());

    [JsonIgnore]
    public ICollection<MessageBriefDto> LastMessages { get; set; }

    public void Mapping(Profile profile)
    {
        Guid currentUserId = Guid.Empty;

        profile.CreateMap<Chat, ChatBriefDto>()
            .ForMember(
                x => x.Guests,
                opts => opts.MapFrom(src => src.Users.Where(x => x.Id != currentUserId).Take(1)))
            .ForMember(
                x => x.LastMessages,
                opts => opts.MapFrom(src => src.Messages.OrderBy(x => x.CreatedBy).Take(1)));
        // profile.CreateMap<Chat, ChatBriefDto>()
        //     .ForMember(
        //         x => x.Guests,
        //         opts => opts.MapFrom(src => src.Users.Where(x => x.Id != currentUserId).Take(1)))
        //     .ForMember(
        //         x => x.LastMessages,
        //         opts => opts.MapFrom(src => src.Messages.OrderBy(x => x.CreatedBy).Take(1)));
    }
}
