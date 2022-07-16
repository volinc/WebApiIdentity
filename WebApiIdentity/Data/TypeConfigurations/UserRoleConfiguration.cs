using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Data.TypeConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasData(new UserRole
        {
            UserId = Seed.SeedUserId,
            RoleId = Seed.AdminRoleId
        });
    }
}