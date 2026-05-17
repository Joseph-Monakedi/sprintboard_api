using SprintboardApi.Domain;

namespace SprintboardApi.Services;

public sealed class AuditLogger
{
    public List<string> Log { get; } = [];

    public void HandleStatusChanged(object? sender, TaskStatusChangedArgs args)
    {
        Log.Add(FormatEntry(args));
    }

    public void LogStatusChange(TaskStatusChangedArgs args)
    {
        Log.Add(FormatEntry(args));
    }

    private static string FormatEntry(TaskStatusChangedArgs args)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{args.TaskId} \"{args.Title}\": {args.OldStatus} → {args.NewStatus}";
    }
}
