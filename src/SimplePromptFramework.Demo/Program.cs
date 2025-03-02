using Microsoft.Extensions.DependencyInjection;
using SimplePromptFramework.Demo.PromptHandlers.Services;

namespace SimplePromptFramework.Demo;

class Program
{
    static async Task Main(string[] args)
    {
        var spf = new Spf(args, o => 
        {
            o.BaseNamespace = "SimplePromptFramework.Demo.PromptHandlers";
            o.Services.AddSingleton<INoteRepository, NoteRepository>();
        });

        await spf.StartAsync();
    }
}

public class CustomExitor : ISpfExitor
{
    public Task<bool> ExitAsync(SpfState state)
    {
        Console.Write("Are you sure you want to exit? (y/n): ");
        return Task.FromResult(Console.ReadLine()?.Trim().ToLower() == "y");
    }
}

public class CustomNoMatchHandler : ISpfNoPromptMatchHandler
{
    public Task<bool> HandleNoMatch(string[] input, SpfState state)
    {
        Console.WriteLine($"Unknown command: {string.Join(" ", input)}");
        return Task.FromResult(false);
    }
}
