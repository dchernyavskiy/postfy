using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Users.Dtos;
using Riok.Mapperly.Abstractions;

namespace Postfy.Services.Network.Messages.Dtos;

public class MessageBriefDtoBase
{
    public Guid Id { get; set; }
    public string Text { get; set; }

    public bool IsWrittenByYou { get; set; }
    public bool IsPost { get; set; }
    public PostBriefDto? Post { get; set; }
    public UserBriefDto User { get; set; }
    public MessageBriefDto? Parent { get; set; }
}

public class MessageBriefDto : MessageBriefDtoBase, IMapWith<Message>
{
    public void Mapping(Profile profile)
    {
        var currentUserId = Guid.Empty;
        profile.CreateMap<Message, MessageBriefDto>()
            .ForMember(
                x => x.IsWrittenByYou,
                opts => opts.MapFrom(src => src.SenderId == currentUserId));
        // profile.CreateMap<Message, MessageBriefDto>()
        //     .ForMember(
        //         x => x.IsWrittenByYou,
        //         opts => opts.MapFrom((src, dest, _, ctx) => src.SenderId == (Guid)ctx.Items["currentUserId"]));
    }
}
