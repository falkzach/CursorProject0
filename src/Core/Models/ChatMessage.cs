using System;
using System.ComponentModel.DataAnnotations;

namespace CursorProject0.Core.Models;

public class ChatMessage
{
    [Required]
    public Guid Id { get; set; }

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

    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public bool DeletedByMod { get; set; }

    [Required]
    public bool EditedByMod { get; set; }

    [Required]
    public bool EditedByUser { get; set; }

    [Required]
    public bool IsModMessage { get; set; }
} 