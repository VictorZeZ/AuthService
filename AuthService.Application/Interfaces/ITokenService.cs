using AuthService.Infrastructure.Entities;
using System.Security.Claims;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user, string deviceInfo);
        Task<bool> ValidateDeviceAsync(ClaimsPrincipal principal);
        Guid? ExtractUserId(ClaimsPrincipal principal);
    }
}
