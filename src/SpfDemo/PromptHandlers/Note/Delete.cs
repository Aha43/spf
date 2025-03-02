using SpfFramework;

namespace SpfDemo.PromptHandlers.Note;

public class Delete : ISpfPromptHandler
{
    public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
    {
        Console.WriteLine($"Last note deleted");
        await Task.CompletedTask;
    }
}
