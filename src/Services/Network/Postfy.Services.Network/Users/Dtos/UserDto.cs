using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Dtos;

public class UserDto : IMapWith<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileName { get; set; }
    public string Bio { get; set; }
    public MediaBriefDto? ProfileImage { get; set; }
    public bool IsFollowed { get; set; }

    public int PostCount { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDto>()
            .ForMember(x => x.PostCount, opts => opts.MapFrom(src => src.Posts.Count()))
            .ForMember(x => x.FollowerCount, opts => opts.MapFrom(src => src.Followers.Count()))
            .ForMember(x => x.FollowingCount, opts => opts.MapFrom(src => src.Followings.Count()))
            .ForMember(
                x => x.IsFollowed,
                opts => opts.MapFrom(
                    (src, dest, isDest, ctx) => src.Followers.Any(x => x.Id == (Guid)ctx.Items["UserId"])));
    }
}
