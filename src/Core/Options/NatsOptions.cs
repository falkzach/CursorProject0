namespace CursorProject0.Core.Options;

public class NatsOptions
{
    public const string SectionName = "NATS";
    
    public string ConnectionString { get; set; } = "nats://localhost:4222";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public int ReconnectWait { get; set; } = 2000;
    public int MaxReconnects { get; set; } = 5;
    public int PingInterval { get; set; } = 20000;
    public int MaxPingsOut { get; set; } = 2;
    public bool Verbose { get; set; }
    public bool Pedantic { get; set; }
} 