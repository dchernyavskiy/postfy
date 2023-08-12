using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using BuildingBlocks.Security.Jwt;
using MongoDB.Driver.Core.Authentication;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Dtos;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Posts.Dtos;

public record PostBriefDtoProxy() : IMapWith<Post>
{
    public Guid Id { get; set; }
    public Guid CurrentUserId { get; set; }
    public string Caption { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Message? Message { get; set; }
    public Guid? MessageId { get; set; }


    public ICollection<Media> Medias { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Reaction> Reactions { get; set; }
    public ICollection<User> Savers { get; set; }
}

public record PostBriefDto : IMapWith<Post>, IMapWith<PostBriefDtoProxy>
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public ICollection<MediaBriefDto> Medias { get; set; }
    public UserDto User { get; set; }
    public bool IsLiked { get; set; }
    public bool HasLike { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime Created { get; set; }
    public ICollection<Comment> Comments { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostBriefDto>()
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentCount, opts => opts.MapFrom(src => src.Comments.Count()))
            .ForMember(
                x => x.Comments,
                opts => opts.MapFrom(src => src.Comments.Take(2)))
            ;

        profile.CreateMap<PostBriefDtoProxy, PostBriefDto>()
            .ForMember(
                x => x.IsLiked,
                opts => opts.MapFrom(
                    src =>
                        src.Reactions.Any(x => x.UserId == src.CurrentUserId && x.IsLiked)))
            .ForMember(x => x.LikeCount, opts => opts.MapFrom(src => src.Reactions.Count(x => x.IsLiked)))
            .ForMember(x => x.CommentCount, opts => opts.MapFrom(src => src.Comments.Count()))
            .ForMember(
                x => x.Comments,
                opts => opts.MapFrom(src => src.Comments.Take(2)))
            ;
    }
}
