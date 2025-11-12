using AuthService.API.GraphQL.InputTypes;
using AuthService.API.GraphQL.Types;
using AuthService.Application.Interfaces;

namespace AuthService.API.GraphQL.Mutations
{
    [GraphQLDescription("Root mutation for user-related operations.")]
    public class UserMutation
    {
        [GraphQLDescription("Registers a new user and returns an authentication token.")]
        public async Task<AuthPayload> RegisterUser(
            [Service] IUserService userService,
            RegisterUserInput input)
        {
            // Call application service with the signature you showed
            string token = await userService.RegisterAsync(
                input.Username,
                input.Email,
                input.Password,
                input.DeviceInfo);

            return new AuthPayload { Token = token };
        }
    }
}
