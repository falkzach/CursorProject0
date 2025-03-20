using System.Text;
using CursorProject0.Core.Connectivity.NATS;
using CursorProject0.Core.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatLogService.Workers;

public class ChatLogWorker : BackgroundService
{
    private readonly INatsListener _natsListener;
    private readonly ILogger<ChatLogWorker> _logger;
    private readonly NatsTopicsOptions _topics;

    public ChatLogWorker(
        INatsListener natsListener,
        ILogger<ChatLogWorker> logger,
        IOptions<NatsTopicsOptions> topics)
    {
        _natsListener = natsListener;
        _logger = logger;
        _topics = topics.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ChatLogWorker starting...");

        // Subscribe to chat messages
        await _natsListener.SubscribeAsync(_topics.ChatMessages, "chat-log-group");

        // Handle incoming messages
        _natsListener.OnMessageReceived += async (data) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(data);
                _logger.LogInformation("Received chat message: {Message}", message);
                // TODO: Add message to database
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
            }
        };

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("ChatLogWorker stopping...");
    }
} 