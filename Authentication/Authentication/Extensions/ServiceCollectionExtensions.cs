using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Extensions;

public static class ServiceCollectionExtensions
{
    // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0
    public static void AddIdentityAuthentication<TDbContext, TUser, TRole>(this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
        where TUser : IdentityUser<Guid>
        where TRole : IdentityRole<Guid>
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(nameof(JwtSettings), jwtSettings);
        services.AddSingleton(jwtSettings);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // false - development only
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = jwtSettings.ClockSkew
                };
            });

        services.AddScoped<JwtSecurityTokenHandler>();

        services.AddIdentityCore<TUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<TRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();
    }
}