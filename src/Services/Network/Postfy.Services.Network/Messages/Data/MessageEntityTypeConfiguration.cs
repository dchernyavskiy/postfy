using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postfy.Services.Network.Messages.Models;
using Postfy.Services.Network.Posts.Models;
using Postfy.Services.Network.Shared.Data;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Messages.Data;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable(nameof(Message).Pluralize().Underscore(), NetworkDbContext.DefaultSchema);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.HasOne(x => x.Parent)
            .WithOne(x => x)
            .HasForeignKey<Message>(x => x.Id);

        builder.HasOne(x => x.Post)
            .WithOne(x => x.Message)
            .HasForeignKey<Post>(x => x.MessageId);

        builder.OwnsMany(
            x => x.Medias,
            n =>
            {
                n.ToTable(("message_" + nameof(Media)).Pluralize().Underscore(), NetworkDbContext.DefaultSchema);

                n.Property(x => x.Id).ValueGeneratedNever();
                n.HasKey(x => x.Id);
                n.HasIndex(x => x.Id).IsUnique();
            });
    }
}
