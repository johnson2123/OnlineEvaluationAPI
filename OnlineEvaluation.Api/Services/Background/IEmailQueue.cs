namespace OnlineEvaluation.Api.Services.Background
{
    public interface IEmailQueue
    {
        ValueTask QueueEmailAsync(EmailJob job);
        ValueTask<EmailJob> DequeueEmailAsync(CancellationToken cancellationToken);
    }
}
