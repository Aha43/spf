using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace SimplePromptFramework.UnitTest;

public class ExitHandlerTests
{
    [Fact]
    public async Task Exit_Can_Be_Overridden()
    {
        // Arrange: Create a mock exit handler that prevents exit
        var mockExitor = new Mock<ISpfExitor>(MockBehavior.Strict);
        mockExitor.Setup(x => x.ExitAsync(It.IsAny<SpfState>())).ReturnsAsync(false);

        var services = new ServiceCollection();
        services.AddSingleton(mockExitor.Object);

        var serviceProvider = services.BuildServiceProvider();
        var spf = new Spf([], o => {
            o.Services = services;
            o.DisableAutoRegistration = true;
        });

        // Act: Directly access `_exitor` (which is now internal)
        var canExit = await spf._exitor!.ExitAsync(new SpfState());

        // Assert: Ensure exit was prevented
        Assert.False(canExit);
        mockExitor.Verify(x => x.ExitAsync(It.IsAny<SpfState>()), Times.Once);
    }
}