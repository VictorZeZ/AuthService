namespace AuthService.Infrastructure.Entities
{
    public class UserDevice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; } = default!;
        public string DeviceInfo { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public User User { get; set; } = default!;
    }
}
