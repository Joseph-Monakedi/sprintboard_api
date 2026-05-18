using SprintboardApi.Domain;
using SprintboardApi.Interfaces;

namespace SprintboardApi.Services;

public sealed class TaskRepository : ITaskRepository
{
    private readonly object _taskLock = new();
    private readonly List<TeamTask> _tasks = [];

    public IReadOnlyList<TeamTask> GetAll()
    {
        lock (_taskLock)
        {
            return _tasks.ToList().AsReadOnly();
        }
    }

    public TeamTask? GetById(int id)
    {
        lock (_taskLock)
        {
            return _tasks.FirstOrDefault(task => task.Id == id);
        }
    }

    public TeamTask Add(TeamTask task)
    {
        lock (_taskLock)
        {
            _tasks.Add(task);
        }

        return task;
    }
}
