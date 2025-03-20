namespace CursorProject0.Core.Options;

public class PostgresOptions
{
    public const string SectionName = "Postgres";
    
    public string Host { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; } = 5432;
    
    public string GetConnectionString()
    {
        return $"Host={Host};Database={Database};Username={Username};Password={Password};Port={Port}";
    }
} 