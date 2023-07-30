using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Core.CQRS.Queries;
using LinqKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Mapping;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomAutoMapper(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies
    )
    {
        builder.Services.AddAutoMapper(
            x =>
            {
                assemblies.ForEach(a => x.AddProfile(new MappingProfile(a)));
            });

        return builder;
    }

    // public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
    //     this IQueryable<TDestination> queryable,
    //     int pageNumber,
    //     int pageSize
    // )
    // where TDestination : class
    //     => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    //
    // public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
    //     this IEnumerable<TDestination> queryable,
    //     int pageNumber,
    //     int pageSize
    // )
    // where TDestination : class
    //     => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);
    //
    // public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
    //     this IQueryable queryable,
    //     IConfigurationProvider configuration
    // )
    // where TDestination : class
    //     => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
}
