using Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.TypeConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasData(new UserRole
            {
                UserId = Seed.AdminUserId,
                RoleId = Seed.AdminRoleId
            },
            new UserRole
            {
                UserId = Seed.AdminUserId,
                RoleId = Seed.CustomerRoleId
            });
    }
}