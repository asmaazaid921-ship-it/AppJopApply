using AppJopApply.Application.Interfaces;

namespace AppJopApply.Infrastruction.Services
{
    /// <summary>
    /// Development stub — logs emails to console. Replace with SMTP/SendGrid in production.
    /// </summary>
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            Console.WriteLine($"[EMAIL] To: {toEmail} | Subject: {subject}");
            Console.WriteLine(body);
            return Task.CompletedTask;
        }
    }
}
