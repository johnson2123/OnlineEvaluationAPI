
using System.Threading.Channels;

namespace OnlineEvaluation.Api.Services.Background
{
    public class BackgroundEmailQueue : IEmailQueue
    {
        private readonly Channel<EmailJob> _queue;

        public BackgroundEmailQueue()
        {
            var options = new BoundedChannelOptions(20000)
            {
                FullMode = BoundedChannelFullMode.Wait,

                SingleWriter = false,

                SingleReader = true
            };

            _queue = Channel.CreateBounded<EmailJob>(options);
        }

        public async ValueTask QueueEmailAsync(EmailJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            await _queue.Writer.WriteAsync(job);
        }

        public async ValueTask<EmailJob> DequeueEmailAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
