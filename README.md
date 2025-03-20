# CursorProject0

A microservices-based .NET 8 application with PostgreSQL, Redis, and NATS integration.

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)

## Getting Started

1. Start the dependency services:
```bash
docker compose --profile deps up -d
```

2. Start all services:
```bash
docker compose up -d
```

The services will be available at:
- ChatLogService: `http://localhost:8081`
- ChatModerationService: `http://localhost:8082`
- EmojiLogService: `http://localhost:8083`
- EmojiFavoritesService: `http://localhost:8084`
- EmojiBombService: `http://localhost:8085`

## Services

### Core Services
- ChatLogService: Logs chat messages and events
- ChatModerationService: Handles chat moderation and filtering
- EmojiLogService: Tracks emoji usage and statistics
- EmojiFavoritesService: Manages user emoji favorites
- EmojiBombService: Handles emoji reactions and effects

### Infrastructure
- PostgreSQL: Database for persistent storage
- Redis: Caching and real-time data
- NATS: Message broker for inter-service communication

## NATS Examples

### Publishing Messages

Using `nats-pub` to publish messages to a topic:

```bash
# Publish a simple message
docker compose exec nats nats-pub test.subject "Hello, NATS!"

# Publish a JSON message
docker compose exec nats nats-pub test.subject '{"message": "Hello, NATS!", "timestamp": "2024-03-19T12:00:00Z"}'

# Publish with reply subject
docker compose exec nats nats-pub test.subject --reply=reply.subject "Hello, NATS! Please reply!"
```

### Subscribing to Messages

Using `nats-sub` to subscribe to topics:

```bash
# Subscribe to a topic
docker compose exec nats nats-sub test.subject

# Subscribe with wildcard
docker compose exec nats nats-sub "test.*"

# Subscribe with queue group
docker compose exec nats nats-sub -q mygroup test.subject
```

## Development

### Running Tests

```bash
# Run tests for all services
dotnet test src/ChatLogService/ChatLogService.csproj
dotnet test src/ChatModerationService/ChatModerationService.csproj
dotnet test src/EmojiLogService/EmojiLogService.csproj
dotnet test src/EmojiFavoritesService/EmojiFavoritesService.csproj
dotnet test src/EmojiBombService/EmojiBombService.csproj

# Run tests in CI environment
docker compose --profile deps up -d
dotnet test src/**/*.csproj
```

### Project Structure

```
CursorProject0/
├── src/
│   ├── Core/                      # Shared library
│   │   ├── Options/              # Common configuration options
│   │   │   ├── PostgresOptions.cs
│   │   │   ├── RedisOptions.cs
│   │   │   └── NatsOptions.cs
│   │   └── Models/               # Shared data models
│   ├── ChatLogService/           # Chat logging service
│   ├── ChatModerationService/    # Chat moderation service
│   ├── EmojiLogService/          # Emoji logging service
│   ├── EmojiFavoritesService/    # Emoji favorites service
│   └── EmojiBombService/         # Emoji bomb service
├── docker-compose.yaml
└── .gitignore
```

## Configuration

Each service uses the following configuration sections from the Core library:

- `Postgres`: Database connection settings
- `Redis`: Redis connection and timeout settings
- `NATS`: Message broker connection and behavior settings

See `appsettings.Development.json` in each service directory for development configuration values. 