using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Configurations
{
    /// <summary>
    /// Configures PostgreSQL database connection for the application.
    /// </summary>
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
