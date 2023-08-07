using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Posts.Dtos;

public record PostBriefDto : IMapWith<Post>
{
    public string Caption { get; set; }
    public ICollection<MediaBriefDto> Medias { get; set; }
    public UserDto User { get; set; }
    public bool IsLiked { get; set; }
    public int LikeCount { get; set; }
    public int CommentLike { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostBriefDto>()
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentLike, opts => opts.MapFrom(src => src.Comments.Count()))
            .ForMember(
                x => x.IsLiked,
                opts =>
                    opts.MapFrom(
                        src =>
                            src.Reactions.FirstOrDefault(x => src.UserId == x.UserId) != null &&
                            src.Reactions.FirstOrDefault(x => src.UserId == x.UserId)!.IsLiked))
            ;
    }
}
