using Microsoft.Extensions.DependencyInjection;
using SpfFramework;

namespace SpfDemo;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        
        // Registering handlers and services
        services.AddSingleton<ISpfExitor, CustomExitor>();
        services.AddSingleton<ISpfNoPromptMatchHandler, CustomNoMatchHandler>();
        services.AddSingleton<PromptHandlers.Note.Create>();
        
        var spf = new Spf(args, services);
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
