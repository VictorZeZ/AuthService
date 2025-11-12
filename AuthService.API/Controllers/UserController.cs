using AuthService.API.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    /// <summary>
    /// Manages user registration, login, profile retrieval, update, and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(
            IUserService userService,
            ITokenService tokenService
            )
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // ---------------- REGISTER ----------------
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var token = await _userService.RegisterAsync(dto.Username, dto.Email, dto.Password, dto.DeviceInfo);
                return CreatedAtAction(nameof(Register), new
                {
                    success = true,
                    message = "User successfully registered.",
                    token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto.UsernameOrEmail, dto.Password, dto.DeviceInfo);
            if (token == null)
                return Unauthorized(new { success = false, message = "Invalid credentials." });

            return Ok(new { success = true, message = "User successfully logged in.", token });
        }

        // ---------------- PROFILE ----------------
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _tokenService.ExtractUserId(User);
            if (userId == null)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            return Ok(new
            {
                success = true,
                message = "Profile retrieved successfully.",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.CreatedAt
                }
            });
        }

        // ---------------- UPDATE ----------------
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userId = _tokenService.ExtractUserId(User);
            if (userId == null)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = Application.Helpers.PasswordHelper.HashPassword(dto.Password);

            await _userService.UpdateAsync(user);

            return Ok(new { success = true, message = "Profile updated successfully." });
        }

        // ---------------- DELETE ----------------
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = _tokenService.ExtractUserId(User);
            if (userId == null)
                return Unauthorized();

            var deleted = await _userService.DeleteAsync(userId.Value);
            if (!deleted)
                return BadRequest(new { success = false, message = "Failed to delete account." });

            return Ok(new { success = true, message = "Account deleted successfully." });
        }
    }
}
