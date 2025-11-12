using AuthService.Infrastructure.Entities;

namespace AuthService.Infrastructure.Repositories
{
    /// <summary>
    /// Defines data access operations for the User entity.
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUsernameAsync(string username);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);

        Task<bool> SaveChangesAsync();
    }
}
