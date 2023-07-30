using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Posts.Dtos;

public class PostBriefDto : IMapWith<Post>
{
    public string Caption { get; set; }
    public ICollection<MediaBriefDto> Medias { get; set; }
    public int LikeCount { get; set; }
    public int CommentLike { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostBriefDto>()
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentLike, opts => opts.MapFrom(src => src.Comments.Count()));
    }
}
