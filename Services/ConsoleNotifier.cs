using SprintboardApi.Domain;
using SprintboardApi.Interfaces;

namespace SprintboardApi.Services;

public sealed class ConsoleNotifier : INotifier
{
    public void Notify(TaskStatusChangedArgs args)
    {
        Console.WriteLine($"[Notify] \"{args.Title}\" is now {args.NewStatus}");
    }
}
