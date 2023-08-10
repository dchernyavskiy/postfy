using BuildingBlocks.Core.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Shared.Contracts;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Shared.Data;

public class NetworkDbContext : EfDbContextBase, INetworkDbContext
{
    public const string DefaultSchema = "network";

    public NetworkDbContext(DbContextOptions<NetworkDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
}
