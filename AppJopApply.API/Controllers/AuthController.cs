using AppJopApply.Application.DTOs.Auth;
using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppJopApply.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Register a new user. A confirmation email will be sent.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(new { Message = "Registration successful. Please check your email to confirm your account." });
        }

        /// <summary>Login with email and password. Returns a JWT token.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Succeeded)
                return Unauthorized(result);
            return Ok(result);
        }

        /// <summary>Confirm email address using the link sent during registration.</summary>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(new { Message = "Email confirmed successfully. You can now log in." });
        }

        /// <summary>Request a password reset link sent to the provided email.</summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok(new { Message = "If the email exists, a password reset link has been sent." });
        }

        /// <summary>Reset the password using the token received via email.</summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(new { Message = "Password has been reset successfully." });
        }

        /// <summary>Assign a role to an existing user (Admin only).</summary>
        [HttpPost("assign-role")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _authService.AssignRoleAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(new { Message = $"Role '{dto.Role}' assigned successfully to user '{dto.Email}'." });
        }

        /// <summary>Get details of the currently authenticated user.</summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var firstName = User.FindFirstValue(JwtRegisteredClaimNames.GivenName);
            var lastName = User.FindFirstValue(JwtRegisteredClaimNames.FamilyName);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToList();

            return Ok(new
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Roles = roles
            });
        }
    }
}
