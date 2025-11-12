using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;
using Moq;

namespace AuthService.Tests.Mocks
{
    /// <summary>
    /// Provides preconfigured mock for IUserRepository.
    /// </summary>
    public static class UserRepositoryMock
    {
        public static Mock<IUserRepository> CreateDefault()
        {
            var mock = new Mock<IUserRepository>();

            // Default: no user exists for any lookup
            mock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            mock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            mock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            return mock;
        }

        public static Mock<IUserRepository> CreateWithExistingUser(User existingUser)
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(r => r.GetByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            mock.Setup(r => r.GetByUsernameAsync(existingUser.Username))
                .ReturnsAsync(existingUser);

            return mock;
        }
    }
}
