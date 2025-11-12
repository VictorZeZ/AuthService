namespace AuthService.API.GraphQL.InputTypes
{
    [GraphQLDescription("Input type for user registration.")]
    public class RegisterUserInput
    {
        [GraphQLDescription("Desired username.")]
        public string Username { get; set; } = string.Empty;

        [GraphQLDescription("User's email address.")]
        public string Email { get; set; } = string.Empty;

        [GraphQLDescription("User's password.")]
        public string Password { get; set; } = string.Empty;

        [GraphQLDescription("Device information (for token/device binding).")]
        public string DeviceInfo { get; set; } = string.Empty;
    }
}
