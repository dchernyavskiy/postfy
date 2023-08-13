using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Posts.Dtos;

public record PostDto : IMapWith<Post>
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public ICollection<MediaBriefDto> Medias { get; set; }
    public UserDto User { get; set; }
    public bool IsLiked { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime Created { get; set; }
    public ICollection<Comment> Comments { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostDto>()
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentCount, opts => opts.MapFrom(src => src.Comments.Count()))
            .AfterMap(
                (src, dest, ctx) =>
                {
                    try
                    {
                        if (ctx.Items.TryGetValue("UserId", out var userId))
                        {
                            dest.IsLiked = src.Reactions.Any(x => x.IsLiked && x.UserId == (Guid)userId);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });
    }
}
