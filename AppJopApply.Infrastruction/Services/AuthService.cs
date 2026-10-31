using AppJopApply.Application.DTOs.Auth;
using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Constants;
using AppJopApply.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppJopApply.Infrastruction.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
                return Fail("Email is already registered.");

            var requestedRole = string.IsNullOrWhiteSpace(dto.Role) ? AppRoles.Candidate : dto.Role.Trim();
            if (requestedRole != AppRoles.Admin && requestedRole != AppRoles.HR && requestedRole != AppRoles.Candidate)
            {
                return Fail($"Invalid role '{requestedRole}'. Allowed roles are: {AppRoles.Admin}, {AppRoles.HR}, {AppRoles.Candidate}.");
            }

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Fail(result.Errors.Select(e => e.Description));

            if (!await _roleManager.RoleExistsAsync(requestedRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(requestedRole));
            }

            await _userManager.AddToRoleAsync(user, requestedRole);

            // Send confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = $"{_configuration["AppSettings:BaseUrl"]}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email",
                $"<p>Please confirm your email by clicking <a href='{confirmLink}'>here</a>.</p>");

            return new AuthResultDto { Succeeded = true, Roles = [requestedRole] };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Fail("Invalid email or password.");

            if (!user.EmailConfirmed)
                return Fail("Please confirm your email before logging in.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            return new AuthResultDto { Succeeded = true, Token = token, Roles = roles };
        }

        public async Task<AuthResultDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Fail("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? new AuthResultDto { Succeeded = true }
                : Fail(result.Errors.Select(e => e.Description));
        }

        public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            // Return success even if user not found (security best practice)
            if (user is null || !user.EmailConfirmed)
                return new AuthResultDto { Succeeded = true };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_configuration["AppSettings:BaseUrl"]}/api/auth/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendEmailAsync(user.Email!, "Reset your password",
                $"<p>Reset your password by clicking <a href='{resetLink}'>here</a>.</p>");

            return new AuthResultDto { Succeeded = true };
        }

        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Fail("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            return result.Succeeded
                ? new AuthResultDto { Succeeded = true }
                : Fail(result.Errors.Select(e => e.Description));
        }

        public async Task<AuthResultDto> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Fail($"User with email '{dto.Email}' was not found.");

            var role = dto.Role.Trim();
            if (role != AppRoles.Admin && role != AppRoles.HR && role != AppRoles.Candidate)
                return Fail($"Invalid role '{role}'. Allowed roles are: {AppRoles.Admin}, {AppRoles.HR}, {AppRoles.Candidate}.");

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            if (await _userManager.IsInRoleAsync(user, role))
                return Fail($"User already has role '{role}'.");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return Fail(result.Errors.Select(e => e.Description));

            var updatedRoles = await _userManager.GetRolesAsync(user);
            return new AuthResultDto { Succeeded = true, Roles = updatedRoles };
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static AuthResultDto Fail(string error) => Fail([error]);
        private static AuthResultDto Fail(IEnumerable<string> errors) =>
            new AuthResultDto { Succeeded = false, Errors = errors };
    }
}
