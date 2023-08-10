using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Contracts;

namespace Postfy.Services.Network.Messages.Data;

public class MessageDataSeeder : IDataSeeder
{
    private sealed class MessageFaker : Faker<Message>
    {
        public MessageFaker(Guid chatId, Guid senderId, Post post)
        {
            CustomInstantiator(
                f =>
                {
                    var isPost = f.Random.Bool(.10f);
                    return new Message()
                           {
                               Text = f.Lorem.Text(),
                               ChatId = chatId,
                               SenderId = senderId,
                               IsPost = isPost,
                               Post = isPost ? post : null
                           };
                });
        }
    }

    private readonly INetworkDbContext _context;

    public MessageDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Messages.AnyAsync()) return;
        var chats = await _context.Chats
                        .Include(x => x.Users)
                        .ToListAsync();
        var posts = await _context.Posts.ToListAsync();
        var faker = new Faker();
        foreach (var chat in faker.Random.ArrayElements(chats.ToArray(), 4))
        {
            foreach (var post in faker.Random.ArrayElements(posts.ToArray(), 2))
            {
                foreach (var user in chat.Users)
                {
                    var message = new MessageFaker(chat.Id, user.Id, post);
                    await _context.Messages.AddAsync(message);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 5;
}
