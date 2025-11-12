namespace AuthService.API.Configurations
{
    /// <summary>
    /// Provides centralized configuration for CORS (Cross-Origin Resource Sharing).
    /// </summary>
    public static class CorsConfiguration
    {
        private const string PolicyName = "AllowAll";

        /// <summary>
        /// Registers a global CORS policy that allows requests from any origin.
        /// </summary>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }

        /// <summary>
        /// Enables the defined CORS policy in the middleware pipeline.
        /// </summary>
        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(PolicyName);
            return app;
        }
    }
}
