using AppJopApply.Application.DTOs.Auth;

namespace AppJopApply.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> ConfirmEmailAsync(string userId, string token);
        Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResultDto> AssignRoleAsync(AssignRoleDto dto);
    }
}
