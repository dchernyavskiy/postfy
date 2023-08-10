using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Data;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Posts.Data;

public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable(nameof(Post).Pluralize().Underscore(), NetworkDbContext.DefaultSchema);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.OwnsMany(
            x => x.Medias,
            n =>
            {
                n.ToTable(("post_" + nameof(Media)).Pluralize().Underscore(), NetworkDbContext.DefaultSchema);

                n.Property(x => x.Id).ValueGeneratedNever();
                n.HasKey(x => x.Id);
                n.HasIndex(x => x.Id).IsUnique();
            });

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Reactions)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
