using AuthService.Application.DependencyInjection;
using AuthService.Infrastructure.DependencyInjection;

namespace AuthService.API.Configurations
{
    /// <summary>
    /// Centralized configuration for registering application and infrastructure services.
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers all project layers (Application & Infrastructure) into the dependency injection container.
        /// </summary>
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure();

            return services;
        }
    }
}
