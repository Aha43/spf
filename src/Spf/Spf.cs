using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SpfFramework
{
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

    public class Spf
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<ISpfPromptHandler> _handlers;
        private readonly ISpfExitor? _exitor;
        private readonly ISpfNoPromptMatchHandler? _noMatchHandler;
        private readonly SpfState _state = new();
        private readonly string _baseNamespace;

        public Spf(string[] args, IServiceCollection services, string baseNamespace = "")
        {
            _baseNamespace = baseNamespace.ToLower();
            var serviceProvider = services.BuildServiceProvider();
            _serviceProvider = serviceProvider;
            _handlers = DiscoverHandlers(serviceProvider);
            _exitor = serviceProvider.GetService<ISpfExitor>();
            _noMatchHandler = serviceProvider.GetService<ISpfNoPromptMatchHandler>();
        }

        private static List<ISpfPromptHandler> DiscoverHandlers(IServiceProvider serviceProvider)
        {
            return [.. AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ISpfPromptHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (ISpfPromptHandler)serviceProvider.GetRequiredService(t))];
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

        private static (string[] path, string[] input) TokenizeInput(string[] tokens)
        {
            var lastIndex = tokens.ToList().FindLastIndex(t => char.IsUpper(t.FirstOrDefault()));
            if (lastIndex == -1) return (tokens, Array.Empty<string>());
            return (tokens[..(lastIndex + 1)], tokens[(lastIndex + 1)..]);
        }

        private bool MatchesHandler(ISpfPromptHandler handler, string[] path)
        {
            var typeName = handler.GetType().Name;
            if (typeName.EndsWith("SpfPromptHandler"))
                typeName = typeName[..^15];

            var namespacePath = handler.GetType().Namespace?.ToLower().Split('.') ?? [];
            if (!string.IsNullOrEmpty(_baseNamespace) && namespacePath.Take(_baseNamespace.Split('.').Length).SequenceEqual(_baseNamespace.Split('.')))
            {
                namespacePath = [.. namespacePath.Skip(_baseNamespace.Split('.').Length)];
            }
            
            var handlerPath = namespacePath.Append(typeName.ToLower()).ToArray();
            return handlerPath.SequenceEqual(path.Select(p => p.ToLower()));
        }
    }
}
