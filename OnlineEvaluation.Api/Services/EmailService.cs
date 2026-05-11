using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }
        public Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            _logger.LogInformation("EmailSErvice (dev): To={To} Subject={Subject} Body={Body}", to, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
