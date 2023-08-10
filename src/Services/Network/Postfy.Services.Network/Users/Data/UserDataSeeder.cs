using Bogus;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Medias.Data;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Data;

public class UserDataSeeder : IDataSeeder
{
    private sealed class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            CustomInstantiator(
                f =>
                {
                    return new User()
                           {
                               FirstName = f.Name.FirstName(),
                               LastName = f.Name.LastName(),
                               ProfileName = f.Internet.UserName(),
                               ProfileImage = new MediaFaker().Generate()
                           };
                });
        }
    }

    private readonly INetworkDbContext _context;

    public UserDataSeeder(INetworkDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        if (await _context.Users.CountAsync() > 2) return;

        var users = new UserFaker().Generate(20);

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public int Order => 1;
}
