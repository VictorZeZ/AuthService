using AuthService.Application.Configurations;
using AuthService.Application.Services;
using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Tests.Unit
{
    /// <summary>
    /// Unit tests for TokenService covering token generation, validation, and claim extraction.
    /// </summary>
    public class TokenServiceTests
    {
        private readonly Mock<IUserDeviceRepository> _userDeviceRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IOptions<JwtConfiguration> _jwtOptions;
        private readonly TokenService _tokenService;

        private readonly JwtConfiguration _jwtConfig = new()
        {
            SecretKey = "1234567890123456789012345678901234567890",
            Issuer = "AuthService",
            Audience = "AuthServiceAPI",
            ExpiryMonths = 1
        };

        public TokenServiceTests()
        {
            _userDeviceRepositoryMock = new Mock<IUserDeviceRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtOptions = Options.Create(_jwtConfig);

            _tokenService = new TokenService(
                _userDeviceRepositoryMock.Object,
                _jwtOptions,
                _unitOfWorkMock.Object
            );
        }

        [Fact(DisplayName = "GenerateTokenAsync_Should_Create_New_Device_And_Return_Valid_JWT")]
        public async Task GenerateTokenAsync_Should_Create_New_Device_And_Return_Valid_JWT()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "user@example.com" };
            _userDeviceRepositoryMock
                .Setup(r => r.GetByUserIdAndDeviceInfoAsync(user.Id, "Device-A"))
                .ReturnsAsync((UserDevice?)null);

            // Act
            var token = await _tokenService.GenerateTokenAsync(user, "Device-A");

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            jwt.Claims.Should().Contain(c => c.Type == "deviceId");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());

            _userDeviceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserDevice>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "GenerateTokenAsync_Should_Update_Existing_Device_When_Found")]
        public async Task GenerateTokenAsync_Should_Update_Existing_Device_When_Found()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var existingDevice = new UserDevice
            {
                UserId = user.Id,
                DeviceId = "dev-001",
                DeviceInfo = "Device-X",
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            };

            _userDeviceRepositoryMock
                .Setup(r => r.GetByUserIdAndDeviceInfoAsync(user.Id, "Device-X"))
                .ReturnsAsync(existingDevice);

            // Act
            var token = await _tokenService.GenerateTokenAsync(user, "Device-X");

            // Assert
            token.Should().NotBeNull();
            existingDevice.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

            _userDeviceRepositoryMock.Verify(r => r.UpdateAsync(existingDevice), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ValidateDeviceAsync_Should_Return_True_When_Device_Is_Valid")]
        public async Task ValidateDeviceAsync_Should_Return_True_When_Device_Is_Valid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deviceId = "dev-xyz";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("deviceId", deviceId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var validDevice = new UserDevice { UserId = userId, DeviceId = deviceId };
            _userDeviceRepositoryMock
                .Setup(r => r.GetValidDeviceAsync(userId, deviceId))
                .ReturnsAsync(validDevice);

            // Act
            var result = await _tokenService.ValidateDeviceAsync(principal);

            // Assert
            result.Should().BeTrue();
            _userDeviceRepositoryMock.Verify(r => r.UpdateAsync(validDevice), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ValidateDeviceAsync_Should_Return_False_When_Device_Not_Found")]
        public async Task ValidateDeviceAsync_Should_Return_False_When_Device_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deviceId = "dev-missing";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("deviceId", deviceId)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _userDeviceRepositoryMock
                .Setup(r => r.GetValidDeviceAsync(userId, deviceId))
                .ReturnsAsync((UserDevice?)null);

            // Act
            var result = await _tokenService.ValidateDeviceAsync(principal);

            // Assert
            result.Should().BeFalse();
            _userDeviceRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserDevice>()), Times.Never);
        }

        [Fact(DisplayName = "ExtractUserId_Should_Return_Guid_When_Claim_Exists")]
        public void ExtractUserId_Should_Return_Guid_When_Claim_Exists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString())
            }));

            // Act
            var result = _tokenService.ExtractUserId(principal);

            // Assert
            result.Should().Be(id);
        }

        [Fact(DisplayName = "ExtractUserId_Should_Return_Null_When_Claim_Missing")]
        public void ExtractUserId_Should_Return_Null_When_Claim_Missing()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = _tokenService.ExtractUserId(principal);

            // Assert
            result.Should().BeNull();
        }
    }
}
