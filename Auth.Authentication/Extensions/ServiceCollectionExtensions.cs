using Auth.Authentication.Handlers;
using Auth.Authentication.Helpers;
using Auth.Authentication.SessionStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Authentication.Extensions;

public static class ServiceCollectionExtensions
{
    // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0
    public static void AddIdentityAuthentication<TDbContext, TUser, TRole>(this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
        where TUser : IdentityUser<long>
        where TRole : IdentityRole<long>
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
                    IssuerSigningKey = SecurityKeyHelper.Create(jwtSettings.TokenPassword),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = jwtSettings.ClockSkew
                };
            });
        
        services.AddIdentityCore<TUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<TRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<AuthenticationService>();
        services.AddSingleton<ISessionStorage, DefaultSessionStorage>();
        services.AddSingleton<Utf8JsonBinarySerializer>();
        services.AddSingleton<CurrentTimeFunc>(() => DateTimeOffset.Now);
        services.AddSingleton<AccessTokenHandler>();
        services.AddSingleton<RefreshTokenHandler>();
    }
}