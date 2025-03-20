# CursorProject0

A .NET 8 Web API project with PostgreSQL, Redis, and NATS integration.

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)

## Getting Started

1. Start the dependency services:
```bash
docker compose --profile deps up -d
```

2. Start the API:
```bash
docker compose up -d
```

The API will be available at `http://localhost:8080`

## Services

- API: .NET 8 Web API
- PostgreSQL: Database
- Redis: Caching
- NATS: Message Broker

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
# Run tests locally
dotnet test src/CursorProject0/CursorProject0.csproj

# Run tests in CI environment
docker compose --profile deps up -d
dotnet test src/CursorProject0/CursorProject0.csproj
```

### Project Structure

```
CursorProject0/
├── src/
│   └── CursorProject0/           # Main API project
│       ├── Options/              # Configuration options
│       ├── Program.cs
│       └── appsettings.json
├── docker-compose.yaml
└── .gitignore
```

## Configuration

The application uses the following configuration sections:

- `Postgres`: Database connection settings
- `Redis`: Redis connection and timeout settings
- `NATS`: Message broker connection and behavior settings

See `appsettings.Development.json` for development configuration values. 