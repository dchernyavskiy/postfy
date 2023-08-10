using AutoMapper;

namespace BuildingBlocks.Abstractions.Mapping;

public interface IMapWith<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).ReverseMap();
}
