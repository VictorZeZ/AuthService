using FluentValidation;
using FluentValidation.AspNetCore;

namespace AuthService.API.Configurations
{
    public static class ValidationConfiguration
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            return services;
        }
    }
}
