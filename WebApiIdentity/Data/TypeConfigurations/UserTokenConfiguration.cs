using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Data.TypeConfigurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.Property(x => x.Location)
            .HasColumnType("geography (point)")
            .HasDefaultValue(Point.Empty)
            .IsRequired();
    }
}