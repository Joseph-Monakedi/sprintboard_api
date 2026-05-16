using TaskStatus = SprintboardApi.Domain.TaskStatus;

namespace SprintboardApi.Contracts;

public sealed record CreateTaskRequest(string Title, string? Description, string? AssignedTo, DateTime? DueDate);

public sealed record AssignRequest(string User);

public sealed record TransitionRequest(TaskStatus NewStatus);
