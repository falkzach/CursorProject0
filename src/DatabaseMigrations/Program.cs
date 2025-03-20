using DbUp;
using DbUp.ScriptProviders;
using Microsoft.Extensions.Configuration;
using Npgsql;

var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false)
    .AddJsonFile("appsettings.User.json", true)
    .AddJsonFile($"appsettings.{env}.json", false)
    .Build();

var baseConnectionString = config.GetConnectionString("Postgres") ?? "";
if (string.IsNullOrEmpty(baseConnectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: connection string required");
    Console.ResetColor();
    return -1;
}
var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString);

var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
var scriptOptions = new FileSystemScriptOptions { IncludeSubDirectories = true };

foreach(var domainScriptsPath in Directory.GetDirectories(scriptsPath)){
    var domain = domainScriptsPath.Split(Path.DirectorySeparatorChar).Last().ToLower();
    connectionStringBuilder.Database = domain;

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Running migrations for {domain}...");
    Console.ResetColor();

    // Create the database if it does not exist
    EnsureDatabase.For.PostgresqlDatabase(connectionStringBuilder.ConnectionString);
    var upgrader = DeployChanges.To.PostgresqlDatabase(connectionStringBuilder.ConnectionString)
        .WithScriptsFromFileSystem(domainScriptsPath, scriptOptions)
        .LogToConsole()
        .Build();
    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(result.Error);
        Console.ResetColor();
        return -1;
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Migrations for {domain} complete");
    Console.ResetColor();
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;
