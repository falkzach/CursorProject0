using ChatLogService.Workers;
using CursorProject0.Core.Connectivity.NATS;
using CursorProject0.Core.Data;
using CursorProject0.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure options
builder.Services.Configure<PostgresOptions>(
    builder.Configuration.GetSection(PostgresOptions.SectionName));
builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<NatsOptions>(
    builder.Configuration.GetSection(NatsOptions.SectionName));
builder.Services.Configure<NatsTopicsOptions>(
    builder.Configuration.GetSection(NatsTopicsOptions.SectionName));

// Register services
builder.Services.AddSingleton<INatsListener, NatsListener>();
builder.Services.AddSingleton<IChatMessageRepository, PostgresChatMessageRepository>();
builder.Services.AddHostedService<ChatLogWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
