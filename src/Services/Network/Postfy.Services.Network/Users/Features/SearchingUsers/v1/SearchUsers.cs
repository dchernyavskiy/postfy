using System.Diagnostics;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS.Queries;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Dtos;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Features.SearchingUsers.v1;

public record SearchUsers(string query) : IQuery<SearchUsersResponse>;

public class SearchUsersHandler : IQueryHandler<SearchUsers, SearchUsersResponse>
{
    private readonly INetworkDbContext _context;
    private readonly IMapper _mapper;
    private readonly ElasticsearchClient _client;

    public SearchUsersHandler(INetworkDbContext context, IMapper mapper, ElasticsearchClient client)
    {
        _context = context;
        _mapper = mapper;
        _client = client;
    }

    public async Task<SearchUsersResponse> Handle(SearchUsers request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
                        .Include(x => x.ProfileImage)
                        .Where(
                            x =>
                                EF.Functions.Like(x.FirstName, $"%{request.query}%") ||
                                EF.Functions.Like(x.LastName, $"%{request.query}%") ||
                                EF.Functions.Like(x.ProfileName, $"%{request.query}%"))
                        .ProjectTo<UserBriefDto>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken: cancellationToken);

        return new SearchUsersResponse(users);
    }
}

public static class Extensions
{
    public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> collection)
    {
        return await Task.Run(collection.ToList);
    }
}
