using SprintboardApi.Domain;
using SprintboardApi.Interfaces;

namespace SprintboardApi.Services;

public sealed class AuditNotifier : INotifier
{
    private readonly AuditLogger _auditLogger;

    public AuditNotifier(AuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public void Notify(TaskStatusChangedArgs args)
    {
        _auditLogger.LogStatusChange(args);
    }
}
