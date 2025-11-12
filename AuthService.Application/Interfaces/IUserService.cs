using AuthService.Infrastructure.Entities;

namespace AuthService.Application.Interfaces
{
    /// <summary>
    /// Defines high-level user operations for the application layer.
    /// </summary>
    public interface IUserService
    {
        Task<string> RegisterAsync(string username, string email, string password, string deviceInfo);

        Task<string?> LoginAsync(string usernameOrEmail, string password, string deviceInfo);

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUsernameAsync(string username);

        Task<User> UpdateAsync(User user);

        Task<bool> DeleteAsync(Guid id);
    }
}
