using SprintboardApi.Interfaces;

namespace SprintboardApi.Domain;

public sealed class TeamTask : IAssignable, ITransitionable, ISchedulable
{
    private static int _nextId;
    private string _title = string.Empty;

    public TeamTask()
    {
        Id = Interlocked.Increment(ref _nextId);
    }

    public int Id { get; }

    public required string Title
    {
        get => _title;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Title cannot be null, empty, or whitespace.", nameof(value));
            }

            _title = value;
        }
    }

    public string? Description { get; init; }
    public string? AssignedTo { get; private set; }
    public DateTime? DueDate { get; init; }
    public TaskStatus Status { get; private set; } = TaskStatus.Backlog;
    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.Today;
    public string Label => AssignedTo ?? "Unassigned";

    public event EventHandler<TaskStatusChangedArgs>? StatusChanged;

    public void Assign(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            throw new ArgumentException("Assigned user cannot be null, empty, or whitespace.", nameof(user));
        }

        AssignedTo = user;
    }

    public void Transition(TaskStatus newStatus)
    {
        if (newStatus == Status)
        {
            return;
        }

        var oldStatus = Status;
        Status = newStatus;

        StatusChanged?.Invoke(this, new TaskStatusChangedArgs(Id, Title, oldStatus, newStatus, AssignedTo));
    }
}
