using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Dtos;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Dtos;

public class UserBriefDto : IMapWith<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileName { get; set; }
    public MediaBriefDto? ProfileImage { get; set; }
}

public class UserBriefDtoWithFollowerCount : IMapWith<User>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileName { get; set; }
    public MediaBriefDto? ProfileImage { get; set; }
    public int FollowerCount { get; set; }
    public bool IsFollowedByYou { get; set; }

    public void Mapping(Profile profile)
    {
        var currentUserId = Guid.NewGuid();
        profile.CreateMap<User, UserBriefDtoWithFollowerCount>()
            .ForMember(x => x.ProfileImage, opts => opts.MapFrom(src => src.ProfileImage))
            .ForMember(
                x => x.IsFollowedByYou,
                opts => opts.MapFrom(src => src.Followers.Any(f => f.Id == currentUserId)))
            .ForMember(x => x.FollowerCount, opts => opts.MapFrom(src => src.Followers.Count()));
    }
}
