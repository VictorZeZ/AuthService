using AuthService.API.Configurations;
using AuthService.API.GraphQL.Mutations;
using AuthService.API.GraphQL.Queries;
using AuthService.API.GraphQL.Types;

namespace AuthService.API.GraphQL
{
    public static class GraphQLConfiguration
    {
        public static IServiceCollection AddGraphQLConfiguration(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddQueryType<UserQuery>()
                .AddMutationType<UserMutation>()
                .AddType<UserType>()
                .AddFiltering()
                .AddSorting()
                .AddProjections()
                .AddGraphQLValidation();

            return services;
        }
    }
}
