using Auth.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetValue<string>("REDIS_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
            services.AddDistributedMemoryCache();
        else
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });
    }

    public static void AddTableStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("CONNECTION_STRING");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}