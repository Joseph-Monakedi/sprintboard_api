using TaskStatus = SprintboardApi.Domain.TaskStatus;

namespace SprintboardApi.Interfaces;

public interface ITransitionable
{
    TaskStatus Status { get; }
    void Transition(TaskStatus newStatus);
}
