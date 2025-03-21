using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursorProject0.Core.Models;

namespace CursorProject0.Core.Data;

public interface IChatMessageRepository
{
    Task<ChatMessage?> GetByIdAsync(Guid id);
    Task<IEnumerable<ChatMessage>> GetByStreamIdAsync(Guid streamId, int? limit = null, DateTime? before = null);
    Task<IEnumerable<ChatMessage>> GetByUserIdAsync(Guid userId, int? limit = null, DateTime? before = null);
    Task<ChatMessage> CreateAsync(CreateChatMessageDto message);
    Task<ChatMessage> UpdateAsync(Guid id, UpdateChatMessageDto update);
    Task<ChatMessage> DeleteAsync(Guid id, DeleteChatMessageDto delete);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountByStreamIdAsync(Guid streamId);
    Task<int> CountByUserIdAsync(Guid userId);
} 