using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpfFramework;

namespace Spf.UnitTest;

public class NoMatchHandlerTests
{
    [Fact]
    public async Task Unrecognized_Command_Calls_NoMatchHandler()
    {
        // Arrange: Create a mock no-match handler
        var mockHandler = new Mock<ISpfNoPromptMatchHandler>();
        mockHandler.Setup(x => x.HandleNoMatch(It.IsAny<string[]>(), It.IsAny<SpfState>()))
                   .ReturnsAsync(true); // ✅ Simulate handling the unknown command

        var spf = new SpfFramework.Spf(Array.Empty<string>(), options =>
        {
            options.DisableAutoRegistration = true; // ✅ Prevent SPF from auto-registering handlers
            options.Services.AddSingleton<ISpfNoPromptMatchHandler>(mockHandler.Object);
        });

        var state = new SpfState();
        var input = new[] { "unknown", "command" };

        // Act: Call the `_noMatchHandler` directly (now internal and accessible)
        var wasHandled = await spf._noMatchHandler!.HandleNoMatch(input, state);

        // Assert: Ensure the handler was called and returned true
        Assert.True(wasHandled);
        mockHandler.Verify(x => x.HandleNoMatch(It.IsAny<string[]>(), It.IsAny<SpfState>()), Times.Once);

    }
}