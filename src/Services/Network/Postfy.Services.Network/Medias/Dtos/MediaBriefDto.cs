using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Shared.Dtos;

public record MediaBriefDto : IMapWith<Media>
{
    public string Url { get; set; }
    public string Type { get; set; }
    public int Position { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Media, MediaBriefDto>()
            .ForMember(
                x => x.Url,
                opts => opts.MapFrom(
                    src => src.Url ??
                           "https://upload.wikimedia.org/wikipedia/commons/8/89/Portrait_Placeholder.png?20170328184010"));
    }
}
