using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postfy.Services.Identity.Shared.Models;

namespace Postfy.Services.Identity.Identity.Data.EntityConfigurations;

public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.ToTable("AccessTokens");

        builder.Property<Guid>("InternalCommandId").ValueGeneratedOnAdd();
        builder.HasKey("InternalCommandId");

        builder.HasIndex(x => new { x.Token, x.UserId }).IsUnique();

        builder.HasOne(rt => rt.ApplicationUser).WithMany(au => au.AccessTokens).HasForeignKey(x => x.UserId);

        builder.Property(rt => rt.Token);

        builder.Property(rt => rt.CreatedAt);
    }
}
