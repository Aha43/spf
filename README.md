# Simple Prompt Framework (SPF)

## Overview
**SPF (Simple Prompt Framework)** is a lightweight and extensible framework for building console-based prompt applications.  
It allows developers to create structured command-line handlers while integrating **dependency injection** and **custom routing**.

### **🚀 Key Features**
- **Automatic command discovery** via `ISpfPromptHandler`
- **Namespace-based routing** for structured commands
- **Dependency injection support** for handlers
- **Custom exit behavior** via `ISpfExitor`
- **Custom handling of unknown commands** via `ISpfNoPromptMatchHandler`
- **Base state loading from JSON (`--state config.json`)**
- **Verbose mode (`--verbose`)** for debugging output

---

## **🔹 Installation**
SPF is available on **NuGet.org**! You can install it using:

```sh
dotnet add package SimplePromptFramework
```

or in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="SimplePromptFramework" Version="1.0.0" />
</ItemGroup>
```

---

## **🚀 Getting Started**

### **1️⃣ Create a Console App**
Run the following command to create a new console project:

```sh
dotnet new console -n MySpfApp
cd MySpfApp
dotnet add package SimplePromptFramework
```

---

### **2️⃣ Modify `Program.cs` to Initialize SPF**
```csharp
using System;
using System.Threading.Tasks;
using SimplePromptFramework;

class Program
{
    static async Task Main(string[] args)
    {
        var spf = new Spf(args);
        await spf.StartAsync();
    }
}
```

🔹 **Now, your app starts SPF and waits for user input!**  
🔹 **Users can type commands, and SPF will process them dynamically.**

---

## **🛠 Creating Prompt Handlers**
SPF discovers **handlers automatically** based on their **namespace and class name**.

### **1️⃣ Implement a Prompt Handler**
Handlers process user commands. Implement `ISpfPromptHandler`:

```csharp
using System;
using System.Threading.Tasks;
using SimplePromptFramework;

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

🔹 Now, users can enter:
```
> Notes Create My first note
```

---

### **2️⃣ Custom Exit Handling**
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

---

### **3️⃣ Handling Unknown Commands**
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

## **⚡ Loading Base State (`--state config.json`)**
SPF supports **loading static data** from a JSON file at startup.

### **1️⃣ Example JSON File**
📄 **config.json**
```json
{
    "Latitude": 51.5074,
    "Longitude": -0.1278,
    "DefaultEmail": "user@example.com"
}
```

### **2️⃣ Pass the JSON File When Starting SPF**
```sh
spf --state config.json
```
Now, the handlers can access these values using `SpfState`:

```csharp
double lat = state.GetState<double>("Latitude");
Console.WriteLine($"Using Latitude: {lat}");
```

✅ **Ensures that constants are available across multiple commands**.

---

## **⚙️ Configuring SPF**
SPF uses the `SpfOptions` class to configure settings. You can pass an options delegate to the `Spf` constructor:

```csharp
var spf = new Spf(args, options =>
{
    options.BaseNamespace = "MyApp.PromptHandlers";
    options.DisableAutoRegistration = true; // Optional: Prevent automatic handler discovery
});
```

### **Available Options**
| Option | Description |
|--------|-------------|
| `BaseNamespace` | The namespace used to resolve handlers (default: `""`) |
| `DisableAutoRegistration` | If `true`, disables automatic discovery of handlers (default: `false`) |
| `Services` | The `IServiceCollection` used for dependency injection |

---

## **🖥️ PowerShell Integration (Planned for v1.0.1)**
We plan to support **PowerShell integration**, allowing users to:
- Run **SPF commands inside PowerShell**.
- Retain **PowerShell history for SPF commands**.
- Use SPF's routing while **leveraging PowerShell's environment**.

---

## **🛠 Roadmap & Future Enhancements**
✔ **Command aliases** (e.g., `t c` for `Tasks Create`).  
✔ **History & command recall** across sessions.  
✔ **Session persistence via `SpfState`**.  
✔ **PowerShell integration (`spf Notes Create` inside PS terminal).**  

---

## **🤝 Contributing**
Pull requests and feedback are welcome! If you encounter issues or have feature requests, submit them via **GitHub Issues**.

---

## **📜 License**
SPF is licensed under the **MIT License**.

Happy coding! 🚀

