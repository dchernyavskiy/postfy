using AutoMapper;
using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Shared.Dtos;

public record MediaBriefDto : IMapWith<Media>
{
    public string Url { get; set; }
    public string Type { get; set; }
    public int Position { get; set; }
}
