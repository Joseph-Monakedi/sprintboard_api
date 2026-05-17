using SprintboardApi.Contracts;
using SprintboardApi.Domain;
using SprintboardApi.Interfaces;
using SprintboardApi.Services;

namespace SprintboardApi.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder AddTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", (ITaskRepository repository) =>
            Results.Ok(repository.GetAll()))
            .WithName("GetTasks")
            .WithSummary("List all tasks");

        group.MapGet("/overdue", (ITaskRepository repository) =>
            Results.Ok(repository.GetAll().Where(task => task.IsOverdue)))
            .WithName("GetOverdueTasks")
            .WithSummary("List overdue tasks");

        group.MapGet("/{id:int}", (int id, ITaskRepository repository) =>
        {
            var task = repository.GetById(id);

            return task is null
                ? Results.NotFound(new { message = $"Task {id} was not found." })
                : Results.Ok(task);
        })
        .WithName("GetTaskById")
        .WithSummary("Get one task");

        group.MapPost("/", (
            CreateTaskRequest request,
            ITaskRepository repository,
            AuditNotifier auditNotifier,
            ConsoleNotifier consoleNotifier,
            CompletionNotifier completionNotifier) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            var task = new TeamTask
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate
            };

            if (!string.IsNullOrWhiteSpace(request.AssignedTo))
            {
                task.Assign(request.AssignedTo);
            }

            SubscribeStatusNotifiers(task, auditNotifier, consoleNotifier, completionNotifier);

            var created = repository.Add(task);

            return Results.Created($"/api/tasks/{created.Id}", created);
        })
        .WithName("CreateTask")
        .WithSummary("Create a task");

        group.MapPatch("/{id:int}/assign", (int id, AssignRequest request, ITaskRepository repository) =>
        {
            if (repository.GetById(id) is not IAssignable assignable)
            {
                return Results.NotFound(new { message = $"Task {id} was not found." });
            }

            try
            {
                assignable.Assign(request.User);
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { message = exception.Message });
            }

            return Results.NoContent();
        })
        .WithName("AssignTask")
        .WithSummary("Assign a task");

        group.MapPatch("/{id:int}/status", (int id, TransitionRequest request, ITaskRepository repository) =>
        {
            if (repository.GetById(id) is not ITransitionable transitionable)
            {
                return Results.NotFound(new { message = $"Task {id} was not found." });
            }

            if (transitionable.Status == request.NewStatus)
            {
                return Results.BadRequest(new { message = $"Task is already {request.NewStatus}." });
            }

            transitionable.Transition(request.NewStatus);

            return Results.NoContent();
        })
        .WithName("TransitionTask")
        .WithSummary("Transition a task status");

        return app;
    }

    private static void SubscribeStatusNotifiers(
        TeamTask task,
        AuditNotifier auditNotifier,
        ConsoleNotifier consoleNotifier,
        CompletionNotifier completionNotifier)
    {
        task.StatusChanged += (_, args) => auditNotifier.Notify(args);
        task.StatusChanged += (_, args) => consoleNotifier.Notify(args);
        task.StatusChanged += (_, args) => completionNotifier.Notify(args);
    }
}
