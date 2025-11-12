namespace AuthService.API.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DeviceInfo { get; set; } = "Unknown Device";
    }
}
