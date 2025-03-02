using SpfFramework;

namespace SpfDemo.PromptHandlers.Note;

public class Create : ISpfPromptHandler
{
    public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
    {
        Console.WriteLine($"Note created: {string.Join(" ", input)}");
        await Task.CompletedTask;
    }
}
