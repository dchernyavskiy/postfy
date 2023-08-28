using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Newtonsoft.Json;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Messages.Dtos;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Chats.Dtos;

public class ChatDto : IMapWith<Chat>
{
    public Guid Id { get; set; }
    public UserBriefDto Guest => Guests.First();

    [JsonIgnore]
    public ICollection<UserBriefDto> Guests { get; set; }

    public ICollection<MessageBriefDto> Messages { get; set; }

    public void Mapping(Profile profile)
    {
        Guid currentUserId = Guid.Empty;
        profile.CreateMap<Chat, ChatDto>()
            .ForMember(
                x => x.Guests,
                opts => opts.MapFrom(src => src.Users.Where(u => u.Id != currentUserId)));
        // (src, dest, _, ctx) => src.Users.Where(u => u.Id != (Guid)ctx.Items["currentUserId"])));
    }
}
