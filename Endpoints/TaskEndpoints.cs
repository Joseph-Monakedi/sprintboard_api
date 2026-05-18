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
        {
            try
            {
                return Results.Ok(repository.GetAll());
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
        })
            .WithName("GetTasks")
            .WithSummary("List all tasks");

        group.MapGet("/overdue", (ITaskRepository repository) =>
        {
            try
            {
                return Results.Ok(repository.GetAll().Where(task => task.IsOverdue));
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
        })
            .WithName("GetOverdueTasks")
            .WithSummary("List overdue tasks");

        group.MapGet("/{id:int}", (int id, ITaskRepository repository) =>
        {
            if (id <= 0)
            {
                return BadRequest("Task id must be greater than zero.");
            }

            try
            {
                var task = repository.GetById(id);

                return task is null
                    ? NotFound(id)
                    : Results.Ok(task);
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
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
            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest("Title is required.");
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
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
        })
        .WithName("CreateTask")
        .WithSummary("Create a task");

        group.MapPatch("/{id:int}/assign", (int id, AssignRequest request, ITaskRepository repository) =>
        {
            if (id <= 0)
            {
                return BadRequest("Task id must be greater than zero.");
            }

            try
            {
                if (repository.GetById(id) is not IAssignable assignable)
                {
                    return NotFound(id);
                }

                assignable.Assign(request.User);

                return Results.NoContent();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
        })
        .WithName("AssignTask")
        .WithSummary("Assign a task");

        group.MapPatch("/{id:int}/status", (int id, TransitionRequest request, ITaskRepository repository) =>
        {
            if (id <= 0)
            {
                return BadRequest("Task id must be greater than zero.");
            }

            if (!Enum.IsDefined(request.NewStatus))
            {
                return BadRequest($"'{request.NewStatus}' is not a valid task status.");
            }

            try
            {
                if (repository.GetById(id) is not ITransitionable transitionable)
                {
                    return NotFound(id);
                }

                if (transitionable.Status == request.NewStatus)
                {
                    return BadRequest($"Task is already {request.NewStatus}.");
                }

                transitionable.Transition(request.NewStatus);

                return Results.NoContent();
            }
            catch (Exception exception)
            {
                return ServerError(exception);
            }
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

    private static IResult BadRequest(string message)
    {
        return Results.BadRequest(new { message });
    }

    private static IResult NotFound(int id)
    {
        return Results.NotFound(new { message = $"Task {id} was not found." });
    }

    private static IResult ServerError(Exception exception)
    {
        return Results.Problem(
            title: "Something went wrong while processing the request.",
            detail: exception.Message,
            statusCode: StatusCodes.Status500InternalServerError);
    }
}
