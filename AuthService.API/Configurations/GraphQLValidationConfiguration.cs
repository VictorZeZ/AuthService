using AuthService.API.Middlewares;
using HotChocolate.Execution.Configuration;

namespace AuthService.API.Configurations
{
    public static class GraphQLValidationConfiguration
    {
        public static IRequestExecutorBuilder AddGraphQLValidation(this IRequestExecutorBuilder builder)
        {
            builder.UseField<ValidationMiddleware>();

            return builder;
        }
    }
}
