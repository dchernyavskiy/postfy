using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Medias.Data;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Posts.Data;

public class PostDataSeeder : IDataSeeder
{
    private sealed class PostFaker : Faker<Post>
    {
        public PostFaker(Guid userId)
        {
            CustomInstantiator(
                (f) =>
                {
                    return new Post()
                           {
                               Caption = f.Lorem.Sentences(2, " "),
                               Medias = new MediaFaker().Generate(f.Random.Int(1, 5)),
                               UserId = userId
                           };
                });
        }
    }

    private readonly INetworkDbContext _context;

    public PostDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Posts.AnyAsync()) return;
        var users = await _context.Users.Select(x => x.Id).ToListAsync();

        foreach (var user in users)
        {
            var posts = new PostFaker(user).Generate(5);
            await _context.Posts.AddRangeAsync(posts);
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 2;
}
