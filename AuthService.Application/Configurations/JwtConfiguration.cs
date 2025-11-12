namespace AuthService.Application.Configurations
{
    public class JwtConfiguration
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiryMonths { get; set; } = 3;
    }
}
