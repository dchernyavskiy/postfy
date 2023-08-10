using System.Collections;
using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Chats.Data;

public class ChatDataSeeder : IDataSeeder
{
    private sealed class ChatFaker : Faker<Chat>
    {
        public ChatFaker(ICollection<User> users)
        {
            CustomInstantiator(
                f =>
                {
                    return new Chat() {Users = users};
                });
        }
    }

    private readonly INetworkDbContext _context;

    public ChatDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Chats.AnyAsync()) return;

        var users = await _context.Users.ToArrayAsync();

        var faker = new Faker();
        foreach (var user1 in users)
        {
            foreach (var user2 in faker.Random.ArrayElements(users.Where(x => x.Id != user1.Id).ToArray()))
            {
                await _context.Chats.AddAsync(new ChatFaker(new List<User>() {user1, user2}));
            }
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 4;
}
