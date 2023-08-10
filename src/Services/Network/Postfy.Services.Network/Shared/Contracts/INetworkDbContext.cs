using BuildingBlocks.Abstractions.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Comments.Models;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Reactions.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Shared.Contracts;

public interface INetworkDbContext : IDbContext
{
    DbSet<Comment> Comments { get; }
    DbSet<Post> Posts { get; }
    DbSet<Reaction> Reactions { get; }
    DbSet<User> Users { get; }
    DbSet<Chat> Chats { get; }
    DbSet<Message> Messages { get; }
}
