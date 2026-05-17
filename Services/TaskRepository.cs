using SprintboardApi.Domain;
using SprintboardApi.Interfaces;

namespace SprintboardApi.Services;

public sealed class TaskRepository : ITaskRepository
{
    private readonly List<TeamTask> _tasks = [];

    public IReadOnlyList<TeamTask> GetAll()
    {
        return _tasks.AsReadOnly();
    }

    public TeamTask? GetById(int id)
    {
        return _tasks.FirstOrDefault(task => task.Id == id);
    }

    public TeamTask Add(TeamTask task)
    {
        _tasks.Add(task);
        return task;
    }
}
