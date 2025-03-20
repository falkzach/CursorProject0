namespace CursorProject0.Core.Options;

public class NatsTopicsOptions
{
    public const string SectionName = "NatsTopics";

    public string ChatMessages { get; set; } = "*.chat.messages";
} 