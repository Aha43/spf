using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace SimplePromptFramework;

public interface ISpfPromptHandler
{
    Task HandlePromptAsync(string[] path, string[] input, SpfState state);
}

public interface ISpfExitor
{
    Task<bool> ExitAsync(SpfState state);
}

public interface ISpfNoPromptMatchHandler
{
    Task<bool> HandleNoMatch(string[] input, SpfState state);
}

public class SpfState
{
    private readonly Dictionary<string, object> _state = [];

    public T? GetState<T>(string key) => _state.TryGetValue(key, out var value) ? (T)value : default;

    public void SetState<T>(string key, T? value)
    {
        if (value == null)
        {
            _state.Remove(key);
        }
        else
        {
            _state[key] = value;
        }
    }

    public void ClearState() => _state.Clear();
}

public class SpfOptions
{
    public string BaseNamespace { get; set; } = string.Empty;
    public bool DisableAutoRegistration { get; set; } = false;
    public IServiceCollection Services { get; set; } = new ServiceCollection();
}

public class Spf
{
    public IServiceProvider ServiceProvider { get; }

    private readonly bool _verbose = false;
    private readonly List<ISpfPromptHandler> _handlers;
    internal readonly ISpfExitor? _exitor;
    internal readonly ISpfNoPromptMatchHandler? _noMatchHandler;
    private readonly SpfState _state;
    private readonly SpfOptions _options = new();

    public Spf(string[] args, Action<SpfOptions>? o = null)
    {
        if (args.Contains("--verbose")) _verbose = true;

        _state = LoadStateFromArgs(args);
        if (_verbose) Console.WriteLine(StateToJson(_state));

        o?.Invoke(_options);

        // Auto-register all handler implementations
        if (!_options.DisableAutoRegistration)
        {
            AutoRegisterHandlers(_options.Services);
            AutoRegisterSingleInstances<ISpfExitor>(_options.Services);
            AutoRegisterSingleInstances<ISpfNoPromptMatchHandler>(_options.Services);
        }

        ServiceProvider = _options.Services.BuildServiceProvider();
        _handlers = DiscoverHandlers(ServiceProvider);
        _exitor = ServiceProvider.GetService<ISpfExitor>();
        _noMatchHandler = ServiceProvider.GetService<ISpfNoPromptMatchHandler>();
    }

    private static List<ISpfPromptHandler> DiscoverHandlers(IServiceProvider serviceProvider)
    {
        return [.. serviceProvider.GetServices<ISpfPromptHandler>()];
    }

    private void AutoRegisterHandlers(IServiceCollection services)
    {
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ISpfPromptHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var handlerType in handlerTypes)
        {
            if (_verbose) Console.WriteLine($"Registering handler: {handlerType.Name}");
            services.AddTransient(typeof(ISpfPromptHandler), handlerType); // Register with interface binding
        }
    }

    private void AutoRegisterSingleInstances<T>(IServiceCollection services) where T : class
    {
        var instanceType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => typeof(T).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        if (instanceType != null)
        {
            if (_verbose) Console.WriteLine($"Registering single instance: {instanceType.Name}");
            services.AddSingleton(typeof(T), instanceType);
        }
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.Write(" > ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                if (_exitor != null && !await _exitor.ExitAsync(_state))
                    continue;
                break;
            }

            var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) continue;

            var (path, cmdInput) = TokenizeInput(tokens);
            var handler = _handlers.FirstOrDefault(h => MatchesHandler(h, path));

            if (handler != null)
            {
                await handler.HandlePromptAsync(path, cmdInput, _state);
            }
            else if (_noMatchHandler != null && await _noMatchHandler.HandleNoMatch(tokens, _state))
            {
                continue;
            }
            else
            {
                Console.WriteLine("Error: Unrecognized command.");
            }
        }
    }

    internal static (string[] path, string[] input) TokenizeInput(string[] tokens)
    {
        if (tokens.Length == 0) return (Array.Empty<string>(), Array.Empty<string>());

        // Assume first two tokens form the command path
        int splitIndex = Math.Min(tokens.Length, 2);

        return (tokens[..splitIndex], tokens[splitIndex..]);
    }

    private bool MatchesHandler(ISpfPromptHandler handler, string[] path)
    {
        if (_verbose) Console.WriteLine($"Checking path: {string.Join(" ", path)}");

        var baseNamespace = _options.BaseNamespace;

        if (_verbose) Console.WriteLine($"  Base namespace: {baseNamespace}");
        if (_verbose) Console.WriteLine($"  Checking handler: {handler.GetType().Name}");

        var typeName = handler.GetType().Name;
        if (typeName.EndsWith("SpfPromptHandler"))
            typeName = typeName[..^15];

        var namespacePath = handler.GetType().Namespace?.ToLower().Split('.') ?? [];
        if (!string.IsNullOrEmpty(baseNamespace) && namespacePath.Take(baseNamespace.Split('.').Length).SequenceEqual(baseNamespace.ToLower().Split('.')))
        {
            namespacePath = namespacePath.Skip(baseNamespace.Split('.').Length).ToArray(); // 🔹 FIXED
        }

        var handlerPath = namespacePath.Append(typeName.ToLower()).ToArray();
        var normalizedPath = path.Select(p => p.ToLower()).ToArray();

        if (_verbose) Console.WriteLine($"  Handler path: {string.Join(" ", handlerPath)}");
        if (_verbose) Console.WriteLine($"  Normalized path: {string.Join(" ", normalizedPath)}");

        return handlerPath.SequenceEqual(normalizedPath);
    }

    private static string? ParseStateFileFromArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--state", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                {
                    return args[i + 1]; // ✅ Valid state file argument
                }
                else
                {
                    Console.WriteLine("Error: Missing or invalid argument for --state. Expected: --state <file.json>");
                    return null; // ✅ Gracefully handle missing argument
                }
            }
        }
        return null;
    }

    private static SpfState LoadStateFromFile(string file)
    {
        var state = new SpfState();
        if (!File.Exists(file))
        {
            Console.WriteLine($"Error: State file not found: {file}");
            return state; // ✅ Gracefully handle missing file
        }
        
        var json = File.ReadAllText(file);
        state.SetState("state", JsonSerializer.Deserialize<Dictionary<string, object>>(json));

        return state;
    }

    private static SpfState LoadStateFromArgs(string[] args)
    {
        var file = ParseStateFileFromArgs(args);
        if (file != null)
        {
            return LoadStateFromFile(file);
        }
        return new SpfState();
    }

    private static JsonSerializerOptions JsonOptions() => new() { WriteIndented = true };

    private static string StateToJson(SpfState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions());
        return json;
    }

}
