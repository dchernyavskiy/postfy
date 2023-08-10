using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Data;

public class FollowDataSeeder : IDataSeeder
{
    private readonly INetworkDbContext _context;

    public FollowDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Users.AnyAsync(x => Enumerable.Any<User>(x.Followings))) return;
        var users = await _context.Users.ToListAsync();

        var faker = new Faker();
        foreach (var user in users)
        {
            var userWithoutCurrent = users.Where(x => x.Id != user.Id).ToArray();
            user.Followings = faker.Random.ArrayElements(userWithoutCurrent, faker.Random.Int(3, 5)).ToList();
            user.Followers = faker.Random.ArrayElements(userWithoutCurrent, faker.Random.Int(3, 5)).ToList();
            _context.Users.Update(user);
        }

        await _context.SaveChangesAsync();
    }

    public int Order => 2;
}
