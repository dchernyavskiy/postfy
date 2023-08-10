using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Comments.Data;

public class CommentDataSeeder : IDataSeeder
{
    private sealed class CommentFaker : Faker<Comment>
    {
        public CommentFaker(Guid postId, Guid userId)
        {
            CustomInstantiator(
                f =>
                {
                    return new Comment() {Text = f.Lorem.Text(), PostId = postId, UserId = userId};
                });
        }
    }

    private readonly INetworkDbContext _context;

    public CommentDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Comments.AnyAsync()) return;

        var posts = await _context.Posts.ToListAsync();
        var users = await _context.Users.ToListAsync();

        foreach (var post in posts)
        {
            foreach (var user in users.Where(x => x.Id != post.UserId))
            {
                var comment = new CommentFaker(post.Id, user.Id).Generate();
                await _context.Comments.AddAsync(comment);
            }
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 3;
}
