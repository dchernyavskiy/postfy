using BuildingBlocks.Abstractions.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Postfy.Services.Identity.Shared.Models;
using Postfy.Services.Shared.Identity.Users.Events.v1.Integration;

namespace Postfy.Services.Identity.Identity.Data;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBus _bus;

    public IdentityDataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IBus bus
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _bus = bus;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    public int Order => 1;

    private async Task SeedRoles()
    {
        if (!await _roleManager.RoleExistsAsync(ApplicationRole.Admin.Name))
            await _roleManager.CreateAsync(ApplicationRole.Admin);

        if (!await _roleManager.RoleExistsAsync(ApplicationRole.User.Name))
            await _roleManager.CreateAsync(ApplicationRole.User);
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByEmailAsync("mehdi@test.com") == null)
        {
            var user = new ApplicationUser
                       {
                           UserName = "mehdi", FirstName = "Mehdi", LastName = "test", Email = "mehdi@test.com",
                       };

            var result = await _userManager.CreateAsync(user, "123456");

            if (result.Succeeded) await _userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name);

            await _bus.Publish(
                new UserRegisteredV1(
                    user.Id,
                    user.Email,
                    user.PhoneNumber,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    user.UserRoles.Select(x => x.Role?.Name).ToList()));
        }

        if (await _userManager.FindByEmailAsync("mehdi2@test.com") == null)
        {
            var user = new ApplicationUser
                       {
                           UserName = "mehdi2", FirstName = "Mehdi", LastName = "Test", Email = "mehdi2@test.com"
                       };

            var result = await _userManager.CreateAsync(user, "123456");

            if (result.Succeeded) await _userManager.AddToRoleAsync(user, ApplicationRole.User.Name);
            await _bus.Publish(
                new UserRegisteredV1(
                    user.Id,
                    user.Email,
                    user.PhoneNumber,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    user.UserRoles.Select(x => x.Role?.Name).ToList()));
        }
    }
}
