namespace AuthService.API.GraphQL.Types
{
    [GraphQLDescription("Payload returned after authentication/registration.")]
    public class AuthPayload
    {
        [GraphQLDescription("JWT access token.")]
        public string Token { get; set; } = string.Empty;
    }
}
