
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services.Background
{
    public class EmailWorkerService : BackgroundService
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailWorkerService> _logger;

        public EmailWorkerService(
            IEmailQueue emailQueue,
            IServiceProvider serviceProvider,
            ILogger<EmailWorkerService> logger)
        {
            _emailQueue = emailQueue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var emailJob = await _emailQueue.DequeueEmailAsync(stoppingToken);

                    _logger.LogInformation("Processing background email job for: {Email}", emailJob.ToEmail);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        await emailService.SendEmailAsync(emailJob.ToEmail, emailJob.Subject, emailJob.Body);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing a background email job.");
                }
            }

            _logger.LogInformation("Background Email Processing Worker Stopped.");
        }
    }
}
