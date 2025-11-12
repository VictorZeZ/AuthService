using AuthService.Infrastructure.Entities;

namespace AuthService.Infrastructure.Repositories
{
    public interface IUserDeviceRepository
    {
        Task<UserDevice?> GetByIdAsync(Guid id);
        Task<UserDevice?> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId);
        Task<UserDevice?> GetByUserIdAndDeviceInfoAsync(Guid userId, string deviceInfo);
        Task<UserDevice?> GetValidDeviceAsync(Guid userId, string deviceId);
        Task AddAsync(UserDevice device);
        Task UpdateAsync(UserDevice device);
        Task DeleteAsync(UserDevice device);
        Task<bool> SaveChangesAsync();
    }
}
