namespace AppJopApply.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
