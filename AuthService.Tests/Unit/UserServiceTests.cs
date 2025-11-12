using AuthService.Application.Exceptions;
using AuthService.Application.Helpers;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;
using AuthService.Tests.Mocks;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.Unit
{
    /// <summary>
    /// Unit tests for UserService covering registration, login, retrieval, and deletion logic.
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = UserRepositoryMock.CreateDefault();
            _tokenServiceMock = TokenServiceMock.CreateDefault();
            _unitOfWorkMock = UnitOfWorkMock.CreateDefault();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_Should_Create_User_When_Data_Is_Valid()
        {
            var token = await _userService.RegisterAsync("john", "john@example.com", "pass123", "device-x");

            token.Should().Be("fake-jwt-token");
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "john@example.com")), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Email_Already_Exists()
        {
            var existing = new User { Email = "john@example.com" };
            var mockRepo = UserRepositoryMock.CreateWithExistingUser(existing);

            var userService = new UserService(
                mockRepo.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object
            );

            var act = async () => await userService.RegisterAsync("john", "john@example.com", "pass123", "device-x");

            await act.Should().ThrowAsync<DuplicateAccountException>();
            mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Username_Already_Exists()
        {
            var existing = new User { Username = "john" };
            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(existing);

            var act = async () => await _userService.RegisterAsync("john", "john@example.com", "pass123", "device-x");

            await act.Should().ThrowAsync<DuplicateAccountException>();
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Token_When_Credentials_Are_Valid()
        {
            var password = "P@ssw0rd";
            var hashed = PasswordHelper.HashPassword(password);
            var user = new User { Username = "john", Email = "john@example.com", PasswordHash = hashed };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync("john@example.com")).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(user, "device-x")).ReturnsAsync("jwt-token");

            var token = await _userService.LoginAsync("john@example.com", password, "device-x");

            token.Should().Be("jwt-token");
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Null_When_Password_Is_Invalid()
        {
            var user = new User
            {
                Username = "john",
                Email = "john@example.com",
                PasswordHash = PasswordHelper.HashPassword("correct-pass")
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync("john@example.com")).ReturnsAsync(user);

            var result = await _userService.LoginAsync("john@example.com", "wrong-pass", "device-x");

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_User_Exists()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

            var result = await _userService.DeleteAsync(id);

            result.Should().BeTrue();
            _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_User_Does_Not_Exist()
        {
            var id = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await _userService.DeleteAsync(id);

            result.Should().BeFalse();
        }
    }
}
