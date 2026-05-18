using SprintboardApi.Domain;

namespace SprintboardApi.Services;

public sealed class AuditLogger
{
    private readonly object _logLock = new();

    public List<string> Log { get; } = [];

    public void HandleStatusChanged(object? sender, TaskStatusChangedArgs args)
    {
        LogStatusChange(args);
    }

    public void LogStatusChange(TaskStatusChangedArgs args)
    {
        lock (_logLock)
        {
            Log.Add(FormatEntry(args));
        }
    }

    private static string FormatEntry(TaskStatusChangedArgs args)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{args.TaskId} \"{args.Title}\": {args.OldStatus} → {args.NewStatus}";
    }
}
