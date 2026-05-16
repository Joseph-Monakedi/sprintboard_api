using SprintboardApi.Domain;

namespace SprintboardApi.Interfaces;

public interface INotifier
{
    void Notify(TaskStatusChangedArgs args);
}
