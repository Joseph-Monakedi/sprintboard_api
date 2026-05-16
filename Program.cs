using SprintboardApi.Endpoints;
using SprintboardApi.Interfaces;
using SprintboardApi.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddSingleton<AuditLogger>();
builder.Services.AddSingleton<AuditNotifier>();
builder.Services.AddSingleton<ConsoleNotifier>();
builder.Services.AddSingleton<CompletionNotifier>();

var app = builder.Build();

app.MapOpenApi();

app.AddTaskEndpoints();

app.Run();
