using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Entities;
using Moq;

namespace AuthService.Tests.Mocks
{
    /// <summary>
    /// Provides preconfigured mock for ITokenService.
    /// </summary>
    public static class TokenServiceMock
    {
        public static Mock<ITokenService> CreateDefault()
        {
            var mock = new Mock<ITokenService>();

            // Always returns a fake token for any user/device
            mock.Setup(t => t.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync("fake-jwt-token");

            return mock;
        }
    }
}
