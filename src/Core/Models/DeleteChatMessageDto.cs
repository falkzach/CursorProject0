using System;
using System.ComponentModel.DataAnnotations;

namespace CursorProject0.Core.Models;

public class DeleteChatMessageDto
{
    [Required]
    public Guid DeletedBy { get; set; }

    [Required]
    public bool DeletedByMod { get; set; }

    public void ApplyTo(ChatMessage chatMessage)
    {
        chatMessage.DeletedAt = DateTime.UtcNow;
        chatMessage.DeletedBy = DeletedBy;
        chatMessage.DeletedByMod = DeletedByMod;
        chatMessage.UpdatedAt = DateTime.UtcNow;
    }
} 
