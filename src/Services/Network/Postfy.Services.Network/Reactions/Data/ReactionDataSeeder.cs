using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Reactions.Data;

public class ReactionDataSeeder : IDataSeeder
{
    private sealed class ReactionFaker : Faker<Reaction>
    {
        public ReactionFaker(Guid postId, Guid userId)
        {
            CustomInstantiator(
                f =>
                {
                    return new Reaction() {IsLiked = f.Random.Bool(), PostId = postId, UserId = userId};
                });
        }
    }

    private readonly INetworkDbContext _context;

    public ReactionDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Reactions.AnyAsync()) return;

        var users = await _context.Users.ToListAsync();
        var posts = await _context.Posts.ToListAsync();

        foreach (var user in users)
        {
            foreach (var post in posts)
            {
                var reaction = new ReactionFaker(post.Id, user.Id).Generate();
                await _context.Reactions.AddAsync(reaction);
            }
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 3;
}
