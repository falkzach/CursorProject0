using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursorProject0.Core.Models;
using CursorProject0.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CursorProject0.Core.Data;

public class PostgresChatMessageRepository : IChatMessageRepository
{
    private readonly string _connectionString;
    private readonly ILogger<PostgresChatMessageRepository> _logger;

    public PostgresChatMessageRepository(
        IOptions<PostgresOptions> options,
        ILogger<PostgresChatMessageRepository> logger)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = options.Value.Host,
            Database = "chatlog",
            Username = options.Value.Username,
            Password = options.Value.Password,
            Port = options.Value.Port
        };
        _connectionString = builder.ConnectionString;
        _logger = logger;
    }

    public async Task<ChatMessage?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT * FROM chat_messages WHERE id = @Id",
            connection);
        command.Parameters.AddWithValue("Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return MapToChatMessage(reader);
    }

    public async Task<IEnumerable<ChatMessage>> GetByStreamIdAsync(Guid streamId, int? limit = null, DateTime? before = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT * FROM chat_messages WHERE stream_id = @StreamId";
        if (before.HasValue)
        {
            query += " AND created_at < @Before";
        }
        query += " ORDER BY created_at DESC";
        if (limit.HasValue)
        {
            query += " LIMIT @Limit";
        }

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("StreamId", streamId);
        if (before.HasValue)
        {
            command.Parameters.AddWithValue("Before", before.Value);
        }
        if (limit.HasValue)
        {
            command.Parameters.AddWithValue("Limit", limit.Value);
        }

        var messages = new List<ChatMessage>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            messages.Add(MapToChatMessage(reader));
        }

        return messages;
    }

    public async Task<IEnumerable<ChatMessage>> GetByUserIdAsync(Guid userId, int? limit = null, DateTime? before = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT * FROM chat_messages WHERE user_id = @UserId";
        if (before.HasValue)
        {
            query += " AND created_at < @Before";
        }
        query += " ORDER BY created_at DESC";
        if (limit.HasValue)
        {
            query += " LIMIT @Limit";
        }

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("UserId", userId);
        if (before.HasValue)
        {
            command.Parameters.AddWithValue("Before", before.Value);
        }
        if (limit.HasValue)
        {
            command.Parameters.AddWithValue("Limit", limit.Value);
        }

        var messages = new List<ChatMessage>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            messages.Add(MapToChatMessage(reader));
        }

        return messages;
    }

    public async Task<ChatMessage> CreateAsync(CreateChatMessageDto message)
    {
        _logger.LogInformation("Creating chat message: UserId={UserId}, StreamId={StreamId}, Time={Time}, Message={Message}, IsModMessage={IsModMessage}",
            message.UserId, message.StreamId, message.Time, message.Message, message.IsModMessage);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            @"INSERT INTO chat_messages (user_id, stream_id, time, message, is_mod_message, created_at, updated_at)
              VALUES (@UserId, @StreamId, @Time, @Message, @IsModMessage, @CreatedAt, @UpdatedAt)
              RETURNING *",
            connection);

        command.Parameters.AddWithValue("UserId", message.UserId);
        command.Parameters.AddWithValue("StreamId", message.StreamId);
        command.Parameters.AddWithValue("Time", message.Time);
        command.Parameters.AddWithValue("Message", message.Message);
        command.Parameters.AddWithValue("IsModMessage", message.IsModMessage);
        command.Parameters.AddWithValue("CreatedAt", DateTime.UtcNow);
        command.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        var result = MapToChatMessage(reader);

        _logger.LogInformation("Created chat message with ID: {Id}", result.Id);
        return result;
    }

    public async Task<ChatMessage> UpdateAsync(Guid id, UpdateChatMessageDto update)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "UPDATE chat_messages SET updated_at = @UpdatedAt";
        if (update.Message != null)
        {
            query += ", message = @Message, edited_by_user = @EditedByUser, edited_by_mod = @EditedByMod";
        }
        if (update.IsModMessage.HasValue)
        {
            query += ", is_mod_message = @IsModMessage";
        }
        query += " WHERE id = @Id RETURNING *";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("Id", id);
        command.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);
        
        if (update.Message != null)
        {
            command.Parameters.AddWithValue("Message", update.Message);
            command.Parameters.AddWithValue("EditedByUser", !update.EditedByMod);
            command.Parameters.AddWithValue("EditedByMod", update.EditedByMod);
        }
        if (update.IsModMessage.HasValue)
        {
            command.Parameters.AddWithValue("IsModMessage", update.IsModMessage.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        return MapToChatMessage(reader);
    }

    public async Task<ChatMessage> DeleteAsync(Guid id, DeleteChatMessageDto delete)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            @"UPDATE chat_messages 
              SET deleted_at = @DeletedAt, deleted_by = @DeletedBy, 
                  deleted_by_mod = @DeletedByMod, updated_at = @UpdatedAt
              WHERE id = @Id RETURNING *",
            connection);

        command.Parameters.AddWithValue("Id", id);
        command.Parameters.AddWithValue("DeletedAt", DateTime.UtcNow);
        command.Parameters.AddWithValue("DeletedBy", delete.DeletedBy);
        command.Parameters.AddWithValue("DeletedByMod", delete.DeletedByMod);
        command.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        return MapToChatMessage(reader);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT EXISTS(SELECT 1 FROM chat_messages WHERE id = @Id)",
            connection);
        command.Parameters.AddWithValue("Id", id);

        return (bool)await command.ExecuteScalarAsync();
    }

    public async Task<int> CountByStreamIdAsync(Guid streamId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM chat_messages WHERE stream_id = @StreamId",
            connection);
        command.Parameters.AddWithValue("StreamId", streamId);

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM chat_messages WHERE user_id = @UserId",
            connection);
        command.Parameters.AddWithValue("UserId", userId);

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static ChatMessage MapToChatMessage(NpgsqlDataReader reader)
    {
        return new ChatMessage
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            StreamId = reader.GetGuid(reader.GetOrdinal("stream_id")),
            Time = reader.GetInt32(reader.GetOrdinal("time")),
            Message = reader.GetString(reader.GetOrdinal("message")),
            DeletedAt = reader.IsDBNull(reader.GetOrdinal("deleted_at")) 
                ? null 
                : reader.GetDateTime(reader.GetOrdinal("deleted_at")),
            DeletedBy = reader.IsDBNull(reader.GetOrdinal("deleted_by")) 
                ? null 
                : reader.GetGuid(reader.GetOrdinal("deleted_by")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
            DeletedByMod = reader.GetBoolean(reader.GetOrdinal("deleted_by_mod")),
            EditedByMod = reader.GetBoolean(reader.GetOrdinal("edited_by_mod")),
            EditedByUser = reader.GetBoolean(reader.GetOrdinal("edited_by_user")),
            IsModMessage = reader.GetBoolean(reader.GetOrdinal("is_mod_message"))
        };
    }
} 
