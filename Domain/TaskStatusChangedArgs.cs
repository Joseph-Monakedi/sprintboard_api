namespace SprintboardApi.Domain;

public sealed class TaskStatusChangedArgs : EventArgs
{
    public TaskStatusChangedArgs(
        int taskId,
        string title,
        TaskStatus oldStatus,
        TaskStatus newStatus,
        string? assignedTo)
    {
        TaskId = taskId;
        Title = title;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        AssignedTo = assignedTo;
    }

    public int TaskId { get; }
    public string Title { get; }
    public TaskStatus OldStatus { get; }
    public TaskStatus NewStatus { get; }
    public string? AssignedTo { get; }
}
