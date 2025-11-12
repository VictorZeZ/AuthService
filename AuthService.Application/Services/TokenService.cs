using AuthService.Application.Configurations;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Entities;
using AuthService.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Application.Services
{
    /// <summary>
    /// Service responsible for generating and validating JWT tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtConfiguration _jwtConfig;

        public TokenService(IUserDeviceRepository userDeviceRepository, IOptions<JwtConfiguration> jwtOptions, IUnitOfWork unitOfWork)
        {
            _userDeviceRepository = userDeviceRepository;
            _unitOfWork = unitOfWork;
            _jwtConfig = jwtOptions.Value;
        }

        /// <summary>
        /// Generates a JWT token for a user and registers the device in the database.
        /// </summary>
        public async Task<string> GenerateTokenAsync(User user, string deviceInfo)
        {
            var expiresAt = DateTime.UtcNow.AddMonths(_jwtConfig.ExpiryMonths);

            // Check if the device already exists for this user
            var existingDevice = await _userDeviceRepository.GetByUserIdAndDeviceInfoAsync(user.Id, deviceInfo);

            string deviceId;
            if (existingDevice != null)
            {
                // Reuse existing deviceId and update expiration
                deviceId = existingDevice.DeviceId;
                existingDevice.ExpiresAt = expiresAt;
                existingDevice.LastUsedAt = DateTime.UtcNow;
                await _userDeviceRepository.UpdateAsync(existingDevice);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                // Create a new device record
                deviceId = Guid.NewGuid().ToString();
                var device = new UserDevice
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    DeviceId = deviceId,
                    DeviceInfo = deviceInfo,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt
                };
                await _userDeviceRepository.AddAsync(device);
                await _unitOfWork.SaveChangesAsync();
            }

            // Build JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim("deviceId", deviceId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// Validates the given JWT token and updates the device's last used timestamp if valid.
        /// </summary>
        public async Task<bool> ValidateDeviceAsync(ClaimsPrincipal principal)
        {
            try
            {
                Console.WriteLine("ValidateDeviceAsync started");

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("UserId claim not found");
                    return false;
                }
                var userId = Guid.Parse(userIdClaim);
                Console.WriteLine($"Extracted userId: {userId}");

                var deviceId = principal.FindFirst("deviceId")?.Value;
                if (string.IsNullOrEmpty(deviceId))
                {
                    Console.WriteLine("DeviceId claim not found");
                    return false;
                }
                Console.WriteLine($"Extracted deviceId: {deviceId}");

                var device = await _userDeviceRepository.GetValidDeviceAsync(userId, deviceId);
                if (device == null)
                {
                    Console.WriteLine("Device not found or invalid");
                    return false;
                }

                device.LastUsedAt = DateTime.UtcNow;
                await _userDeviceRepository.UpdateAsync(device);
                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine("Device updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ValidateDeviceAsync exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public Guid? ExtractUserId(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }
}
