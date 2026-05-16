using SprintboardApi.Domain;

namespace SprintboardApi.Interfaces;

public interface ITaskRepository
{
    IReadOnlyList<TeamTask> GetAll();
    TeamTask? GetById(int id);
    TeamTask Add(TeamTask task);
}
