using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using BuildingBlocks.Security.Jwt;
using MongoDB.Driver.Core.Authentication;
using Newtonsoft.Json;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Posts.Features.GettingPosts.v1;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Posts.Dtos;

public record PostBriefDtoBase
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public ICollection<MediaBriefDto> Medias { get; set; }
    public UserBriefDto User { get; set; }
    public bool IsLiked { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime Created { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

public record PostBriefDto : PostBriefDtoBase, IMapWith<Post>
{
    public void Mapping(Profile profile)
    {
        Guid currentUserId = Guid.Empty;

        profile.CreateMap<Post, PostBriefDto>()
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentCount, opts => opts.MapFrom(src => src.Comments.Count()))
            .ForMember(
                x => x.Comments,
                opts => opts.MapFrom(src => src.Comments.Take(2)))
            .ForMember(
                x => x.IsLiked,
                opts => opts.MapFrom(src => src.Reactions.Any(x => x.IsLiked && x.UserId == currentUserId)));
    }
}
