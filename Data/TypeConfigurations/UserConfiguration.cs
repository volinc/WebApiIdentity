using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Data.TypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        //builder.Property(x => x.FirstName);
        //builder.Property(x => x.GivenName);
        //builder.Property(x => x.UpdatedAt);
        //builder.Property(x => x.CreatedAt);

        builder.HasData(new User
        {
            Id = Seed.SeedUserId,
            UserName = "Admin",
            Email = "admin@go.offline",
            FirstName = null,
            GivenName = null,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            PasswordHash = string.Empty,
            UpdatedAt = Seed.CreatedAt,
            CreatedAt = Seed.CreatedAt
        });
    }
}