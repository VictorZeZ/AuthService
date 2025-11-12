using AuthService.Application.Exceptions;
using AuthService.Application.Helpers;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;

namespace AuthService.Application.Services
{
    /// <summary>
    /// Implements user-related business logic such as registration, login, and account deletion.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> RegisterAsync(string username, string email, string password, string deviceInfo)
        {
            // Check if the email is already registered
            var existingEmailUser = await _userRepository.GetByEmailAsync(email);
            if (existingEmailUser != null)
                throw new DuplicateAccountException();

            // Check if the username is already taken
            var existingUsernameUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUsernameUser != null)
                throw new DuplicateAccountException();

            // Hash the password securely
            var hashedPassword = PasswordHelper.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                var token = await _tokenService.GenerateTokenAsync(user, deviceInfo);

                await _unitOfWork.CommitTransactionAsync(transaction);
                return token;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(transaction);
                throw;
            }
        }

        public async Task<string?> LoginAsync(string usernameOrEmail, string password, string deviceInfo)
        {
            // Try to find by email first, then by username
            var user = await _userRepository.GetByEmailAsync(usernameOrEmail)
                       ?? await _userRepository.GetByUsernameAsync(usernameOrEmail);

            if (user == null)
                return null;

            // Check password
            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                return null;

            var token = await _tokenService.GenerateTokenAsync(user, deviceInfo);

            return token;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            return await _userRepository.SaveChangesAsync();
        }
    }
}
