using SprintboardApi.Domain;
using SprintboardApi.Interfaces;
using TaskStatus = SprintboardApi.Domain.TaskStatus;

namespace SprintboardApi.Services;

public sealed class CompletionNotifier : INotifier
{
    public void Notify(TaskStatusChangedArgs args)
    {
        if (args.NewStatus != TaskStatus.Done)
        {
            return;
        }

        var completedBy = args.AssignedTo ?? "someone";
        Console.WriteLine($"✓ \"{args.Title}\" marked as Done by {completedBy}");
    }
}
