# Simple Prompt Framework (SPF)

## Overview
SPF (**Simple Prompt Framework**) is a lightweight and extensible framework for building console-based prompt applications. It allows developers to create structured command-line handlers without the complexity of traditional CLI parsers.

### Key Features:
- **Automatic command discovery** via `ISpfPromptHandler`
- **Namespace-based routing** for structured commands
- **Dependency injection support** for handlers
- **Custom exit behavior** via `ISpfExitor`
- **Custom handling of unknown commands** via `ISpfNoPromptMatchHandler`
- **Verbose mode (`--verbose`)** for debugging output

---

## Getting Started

### 1. Clone the Repository
Since SPF is not yet published as a NuGet package, you need to clone the repository and reference it in your project.

1. **Clone the SPF repository**:
   ```sh
   git clone <your-repo-url>
   cd spf-framework
   ```
2. **Create a new console project**:
   ```sh
   dotnet new console -n MySpfApp
   cd MySpfApp
   ```
3. **Add a reference to the SPF project**:
   ```sh
   dotnet add reference ../spf-framework/SpfFramework.csproj
   ```
4. **Modify `Program.cs` to initialize SPF**:
   ```csharp
   using System;
   using System.Threading.Tasks;
   using Microsoft.Extensions.DependencyInjection;
   using SpfFramework;

   class Program
   {
       static async Task Main(string[] args)
       {
           var services = new ServiceCollection();
           var spf = new Spf(args, services, baseNamespace: "MySpfApp.PromptHandlers");
           await spf.StartAsync();
       }
   }
   ```

---

## Creating Handlers

### **1. Implementing a Prompt Handler**
Handlers process user commands. Implement `ISpfPromptHandler`:

```csharp
using System;
using System.Threading.Tasks;
using SpfFramework;

namespace MySpfApp.PromptHandlers.Notes
{
    public class Create : ISpfPromptHandler
    {
        public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
        {
            Console.WriteLine($"Note created: {string.Join(" ", input)}");
            await Task.CompletedTask;
        }
    }
}
```

Now, users can enter:
```
> Notes Create My first note
```

### **2. Custom Exit Handling**
To override exit behavior, implement `ISpfExitor`:
```csharp
public class CustomExitor : ISpfExitor
{
    public async Task<bool> ExitAsync(SpfState state)
    {
        Console.Write("Are you sure you want to exit? (y/n): ");
        return Console.ReadLine()?.Trim().ToLower() == "y";
    }
}
```

### **3. Handling Unknown Commands**
To customize handling of unrecognized commands, implement `ISpfNoPromptMatchHandler`:
```csharp
public class CustomNoMatchHandler : ISpfNoPromptMatchHandler
{
    public async Task<bool> HandleNoMatch(string[] input, SpfState state)
    {
        Console.WriteLine($"Unknown command: {string.Join(" ", input)}");
        return false;
    }
}
```

---

## Command Routing
SPF uses **namespace-based routing**, meaning command names are derived from class names and their namespace structure. Example:
```csharp
namespace MyApp.PromptHandlers.Tasks;
public class Complete : ISpfPromptHandler { ... }
```
Command to trigger it:
```
> Tasks Complete my task
```

To avoid users needing to type long paths (e.g., `MyApp PromptHandlers Tasks Complete`), SPF allows specifying a **base namespace**:
```csharp
var spf = new Spf(args, services, baseNamespace: "MyApp.PromptHandlers");
```
Now, `Tasks Complete my task` works directly.

---

## Verbose Mode
Enable debug logging by passing `--verbose` when launching the app:
```sh
MySpfApp --verbose
```
This prints extra details, like handler registration.

---

## Roadmap & Future Enhancements
- **Command aliases** (e.g., `t c` for `Tasks Create`)
- **History & command recall**
- **Session persistence via `SpfState`**

---

## Contributing
Pull requests and feedback are welcome! If you encounter issues or have feature requests, submit them via GitHub Issues.

Happy coding! 🚀
