using System;
using System.ComponentModel.DataAnnotations;

namespace CursorProject0.Core.Models;

public class CreateChatMessageDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid StreamId { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Time { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public bool IsModMessage { get; set; }

    public ChatMessage ToChatMessage()
    {
        return new ChatMessage
        {
            UserId = UserId,
            StreamId = StreamId,
            Time = Time,
            Message = Message,
            IsModMessage = IsModMessage,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
} 
