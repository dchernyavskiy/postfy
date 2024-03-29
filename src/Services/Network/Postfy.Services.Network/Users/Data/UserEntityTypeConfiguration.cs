using BuildingBlocks.Core.Persistence.EfCore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postfy.Services.Network.Chats.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Data;
using Postfy.Services.Network.Shared.Models;
using Postfy.Services.Network.Users.Models;

namespace Postfy.Services.Network.Users.Data;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User).Pluralize().Underscore(), NetworkDbContext.DefaultSchema);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.HasMany(x => x.Followers)
            .WithMany(x => x.Followings)
            .UsingEntity<Subscription>(
                nameof(Subscription).Pluralize().Underscore(),
                r => r.HasOne(x => x.Follower)
                    .WithMany(x => x.FollowerSubscriptions)
                    .HasForeignKey(x => x.FollowerId)
                    .OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne(x => x.Following)
                    .WithMany(x => x.FollowingSubscriptions)
                    .HasForeignKey(x => x.FollowingId)
                    .OnDelete(DeleteBehavior.Cascade),
                typeBuilder =>
                {
                    typeBuilder.HasKey(x => new {x.FollowerId, x.FollowingId});
                });

        builder.HasMany(x => x.Reactions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SavedPosts)
            .WithMany(x => x.Savers)
            .UsingEntity(
                "post_user",
                l => l.HasOne(typeof(Post)).WithMany().HasForeignKey("post_id").HasPrincipalKey(nameof(Post.Id)),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("user_id").HasPrincipalKey(nameof(User.Id)),
                j => j.HasKey("post_id", "user_id"));

        builder.HasMany(x => x.Chats)
            .WithMany(x => x.Users)
            .UsingEntity(
                "chat_user",
                l => l.HasOne(typeof(Chat)).WithMany().HasForeignKey("chat_id").HasPrincipalKey(nameof(Chat.Id)),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("user_id").HasPrincipalKey(nameof(User.Id)),
                j => j.HasKey("chat_id", "user_id"));

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Sender)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(
            x => x.ProfileImage
            // ,
            //  n =>
            //  {
            //      n.Property(x => x.Id).HasColumnName(nameof(Media.Id).Underscore());
            //      n.Property(x => x.Url).HasColumnName(nameof(Media.Url).Underscore());
            //      n.Property(x => x.Url).HasColumnName(nameof(Media.Url).Underscore());
            //      n.Property(x => x.Type).HasColumnName(nameof(Media.Type).Underscore());
            //      n.Property(x => x.Position).HasColumnName(nameof(Media.Position).Underscore());
            //  }
        );
        builder.OwnsOne(x => x.NotificationSettings);
        builder.OwnsOne(x => x.PrivacySettings);

        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
        builder.Property(x => x.ProfileName).HasMaxLength(100);

        builder.Property(x => x.SignupDate).HasDefaultValueSql(EfConstants.DateAlgorithm);
    }
}
