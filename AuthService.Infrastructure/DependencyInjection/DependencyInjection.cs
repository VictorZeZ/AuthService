using AuthService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Registers application and infrastructure dependencies for the DI container.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            return services;
        }
    }
}
