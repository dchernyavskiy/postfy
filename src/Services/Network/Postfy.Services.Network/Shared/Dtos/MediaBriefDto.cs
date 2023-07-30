using BuildingBlocks.Abstractions.Mapping;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Shared.Dtos;

public record MediaBriefDto
(
    string Url,
    string Type,
    int Position
) : IMapWith<Media>;
