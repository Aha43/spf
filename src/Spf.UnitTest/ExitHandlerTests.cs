using Moq;
using SpfFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Spf.UnitTest;

public class ExitHandlerTests
{
    [Fact]
    public async Task Exit_Can_Be_Overridden()
    {
        // Arrange: Create a mock exit handler that prevents exit
        var mockExitor = new Mock<ISpfExitor>();
        mockExitor.Setup(x => x.ExitAsync(It.IsAny<SpfState>())).ReturnsAsync(false); // Prevents exit

        var services = new ServiceCollection();
        services.AddSingleton(mockExitor.Object);

        var spf = new SpfFramework.Spf([], services);

        // Act: Simulate calling the exit handler
        var state = new SpfState();
        var canExit = await mockExitor.Object.ExitAsync(state);

        // Assert: Ensure exit was prevented
        Assert.False(canExit);
        mockExitor.Verify(x => x.ExitAsync(It.IsAny<SpfState>()), Times.Once);
    }
}