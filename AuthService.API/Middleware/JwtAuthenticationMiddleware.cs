using AuthService.Application.Interfaces;
using AuthService.Application.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.API.Middleware
{
    public static class JwtAuthenticationMiddleware
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            // JwtConfiguration must already be configured in DI
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Token validation parameters will be read from DI during request
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception?.ToString() ?? "No exception details available");
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        if (context.Principal == null)
                        {
                            context.Fail("Principal is null.");
                            return;
                        }

                        // Resolve TokenService and JwtConfiguration from DI
                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                        var jwtConfig = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtConfiguration>>().Value;

                        // Configure token validation parameters
                        context.Options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtConfig.Issuer,
                            ValidAudience = jwtConfig.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                            ClockSkew = TimeSpan.Zero
                        };

                        // Validate device via TokenService
                        var isValid = await tokenService.ValidateDeviceAsync(context.Principal);
                        if (!isValid)
                        {
                            context.Fail("Invalid token or device not registered.");
                        }
                    }
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}
