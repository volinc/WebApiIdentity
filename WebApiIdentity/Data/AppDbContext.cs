using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Data;

public class AppDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public const string ConnectionString = "Server=localhost; Database=gooffline; Username=postgres; Password=pass;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseNpgsql(ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var assembly = Assembly.GetExecutingAssembly();
        builder.ApplyConfigurationsFromAssembly(assembly);
    }
}

