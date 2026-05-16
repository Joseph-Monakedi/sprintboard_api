namespace SprintboardApi.Interfaces;

public interface ISchedulable
{
    DateTime? DueDate { get; }
    bool IsOverdue { get; }
}
