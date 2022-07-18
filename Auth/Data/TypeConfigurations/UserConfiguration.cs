using Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.TypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        //builder.Property(x => x.FirstName);
        //builder.Property(x => x.GivenName);
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        const string name = "admin";
        const string email = "vol.inc@gmail.com";

        builder.HasData(new User
        {
            Id = Seed.AdminUserId,
            UserName = name,
            NormalizedUserName = name.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            SecurityStamp = "V4HRMVYXVPMX72WPRZMDMOU3J7RRWGGG",
            PasswordHash = "AQAAAAEAACcQAAAAECpqxvmiOvp/DB0zEV6wvDRHAnkT+MjXsKFG+VWZBiHwA7xD2KOgNzNuh1CGUldifw==",
            ConcurrencyStamp = "c56d27cd-0585-4c55-9f4c-67f6e7c9f5f6",
            FirstName = null,
            GivenName = null,
            UpdatedAt = Seed.CreatedAt,
            CreatedAt = Seed.CreatedAt
        });
    }
}