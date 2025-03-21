using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CursorProject0.Core.Connectivity.NATS;
using CursorProject0.Core.Options;
using Microsoft.Extensions.Options;
using NATS.Client;

namespace CursorProject0.Core.Connectivity.NATS;

public class NatsListener : INatsListener
{
    private readonly IConnection _connection;
    private readonly ConcurrentDictionary<string, IAsyncSubscription> _subscriptions = new();
    private readonly NatsOptions _options;

    public event Func<byte[], Task>? OnMessageReceived;

    public NatsListener(IOptions<NatsOptions> options)
    {
        _options = options.Value;
        var factory = new ConnectionFactory();
        _connection = factory.CreateConnection(_options.ConnectionString);
    }

    public async Task SubscribeAsync(string subject, string? queueGroup = null)
    {
        if (_subscriptions.ContainsKey(subject))
        {
            return;
        }

        var subscription = queueGroup != null
            ? _connection.SubscribeAsync(subject, queueGroup)
            : _connection.SubscribeAsync(subject);

        subscription.MessageHandler += async (sender, args) =>
        {
            if (OnMessageReceived != null)
            {
                await OnMessageReceived(args.Message.Data);
            }
        };

        subscription.Start();
        _subscriptions.TryAdd(subject, subscription);
    }

    public async Task UnsubscribeAsync(string subject)
    {
        if (_subscriptions.TryRemove(subject, out var subscription))
        {
            subscription.Unsubscribe();
            await Task.CompletedTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var subscription in _subscriptions.Values)
        {
            subscription.Unsubscribe();
        }

        _connection.Close();
        await Task.CompletedTask;
    }
} 
