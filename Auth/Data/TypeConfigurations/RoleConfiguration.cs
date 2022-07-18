using Auth.Constants;
using Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.TypeConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasData(new Role
            {
                Id = Seed.AdminRoleId,
                Name = Roles.Admin,
                NormalizedName = Roles.Admin
            },
            new Role
            {
                Id = Seed.CustomerRoleId,
                Name = Roles.Customer,
                NormalizedName = Roles.Customer
            });
    }
}