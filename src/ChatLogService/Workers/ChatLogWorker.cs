using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CursorProject0.Core.Connectivity.NATS;
using CursorProject0.Core.Data;
using CursorProject0.Core.Models;
using CursorProject0.Core.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatLogService.Workers;

public class ChatLogWorker : BackgroundService
{
    private readonly INatsListener _natsListener;
    private readonly IChatMessageRepository _repository;
    private readonly ILogger<ChatLogWorker> _logger;
    private readonly NatsTopicsOptions _topics;
    private readonly JsonSerializerOptions _jsonOptions;

    public ChatLogWorker(
        INatsListener natsListener,
        IChatMessageRepository repository,
        ILogger<ChatLogWorker> logger,
        IOptions<NatsTopicsOptions> topics)
    {
        _natsListener = natsListener;
        _repository = repository;
        _logger = logger;
        _topics = topics.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = 
            { 
                new JsonStringEnumConverter()
            }
        };
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
                _logger.LogInformation("Raw message received: {Message}", message);

                var chatMessage = JsonSerializer.Deserialize<CreateChatMessageDto>(message, _jsonOptions);
                if (chatMessage == null)
                {
                    _logger.LogWarning("Failed to deserialize chat message");
                    return;
                }
                _logger.LogInformation("Deserialized chat message: {ChatMessage}", chatMessage);

                var savedMessage = await _repository.CreateAsync(chatMessage);
                _logger.LogInformation("Saved chat message with ID: {Id}", savedMessage.Id);
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
