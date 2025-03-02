namespace Spf.UnitTest;

using Xunit;
using SpfFramework;

public class TokenizationTests
{
    private static readonly string[] expected1 = ["notes", "create"];
    private static readonly string[] expected2 = ["my", "note"];
    

    [Fact]
    public void Tokenizes_Simple_Command_Correctly()
    {
        var input = new[] { "notes", "create", "my", "note" };
        var (path, args) = Spf.TokenizeInput(input);

        Assert.Equal(expected1, path);
        Assert.Equal(expected2, args);
    }

    private static readonly string[] expected3 = ["help"];

    [Fact]
    public void Handles_Single_Word_Command()
    {
        var input = new[] { "help" };
        var (path, args) = Spf.TokenizeInput(input);

        Assert.Equal(expected3, path);
        Assert.Empty(args);
    }
}
