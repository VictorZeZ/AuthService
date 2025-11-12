using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;

namespace AuthService.API.GraphQL.Queries
{
    [GraphQLDescription("Root query for user-related operations.")]
    public class UserQuery
    {
        [GraphQLDescription("Returns all registered users.")]
        public async Task<IEnumerable<User>> GetUsers([Service] IUserRepository userRepository)
        {
            return await userRepository.GetAllAsync();
        }

        [GraphQLDescription("Returns a single user by ID.")]
        public async Task<User?> GetUserById([Service] IUserRepository userRepository, Guid id)
        {
            return await userRepository.GetByIdAsync(id);
        }
    }
}
