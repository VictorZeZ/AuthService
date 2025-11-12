namespace AuthService.API.DTOs
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DeviceInfo { get; set; } = "Unknown Device";
    }
}
