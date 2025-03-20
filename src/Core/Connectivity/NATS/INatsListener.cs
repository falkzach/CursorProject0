using System;
using System.Threading.Tasks;

namespace CursorProject0.Core.Connectivity.NATS;

public interface INatsListener : IAsyncDisposable
{
    /// <summary>
    /// Subscribes to a NATS subject and starts listening for messages
    /// </summary>
    /// <param name="subject">The NATS subject to subscribe to</param>
    /// <param name="queueGroup">Optional queue group for load balancing</param>
    /// <returns>A task that completes when the subscription is established</returns>
    Task SubscribeAsync(string subject, string? queueGroup = null);

    /// <summary>
    /// Unsubscribes from a NATS subject
    /// </summary>
    /// <param name="subject">The NATS subject to unsubscribe from</param>
    /// <returns>A task that completes when the unsubscription is complete</returns>
    Task UnsubscribeAsync(string subject);

    /// <summary>
    /// Event that is raised when a message is received
    /// </summary>
    event Func<byte[], Task> OnMessageReceived;
} 