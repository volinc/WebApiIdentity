using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiIdentity.Domain.Identity;

namespace WebApiIdentity.Extensions;

public static class ServiceCollectionExtensions
{
    // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0
    public static void AddAuthentication<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                configuration.Bind("JwtSettings", options);
            });

        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();
    }
}