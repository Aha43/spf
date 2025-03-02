using Microsoft.Extensions.DependencyInjection;
using SpfDemo.PromptHandlers.Services;
using SpfFramework;

namespace SpfDemo;

class Program
{
    static async Task Main(string[] args)
    {
        var spf = new Spf(args, o => 
        {
            o.BaseNamespace = "SpfDemo.PromptHandlers";
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
