using System;
using System.ComponentModel.DataAnnotations;

namespace CursorProject0.Core.Models;

public class UpdateChatMessageDto
{
    [MaxLength(500)]
    public string? Message { get; set; }
    public bool? IsModMessage { get; set; }
    [Required]
    public bool EditedByMod { get; set; }

    public void ApplyTo(ChatMessage chatMessage)
    {
        if (Message != null)
        {
            chatMessage.Message = Message;
            chatMessage.EditedByUser = !EditedByMod;
            chatMessage.EditedByMod = EditedByMod;
        }

        if (IsModMessage.HasValue)
        {
            chatMessage.IsModMessage = IsModMessage.Value;
        }

        chatMessage.UpdatedAt = DateTime.UtcNow;
    }
} 