namespace Spf.UnitTest;

using Xunit;
using SpfFramework;

public class SpfStateTests
{
    [Fact]
    public void Can_Store_And_Retrieve_State()
    {
        var state = new SpfState();
        state.SetState("username", "Alice");

        Assert.Equal("Alice", state.GetState<string>("username"));
    }

    [Fact]
    public void Can_Override_Existing_State_Value()
    {
        var state = new SpfState();
        state.SetState("counter", 1);
        state.SetState("counter", 2);

        Assert.Equal(2, state.GetState<int>("counter"));
    }

    [Fact]
    public void Can_Remove_State_Value()
    {
        var state = new SpfState();
        state.SetState("username", "Alice");
        state.SetState<string>("username", null);

        Assert.Null(state.GetState<string>("username"));
    }

    [Fact]
    public void Can_Clear_State()
    {
        var state = new SpfState();
        state.SetState("username", "Alice");
        state.SetState("counter", 10);
        
        state.ClearState();

        Assert.Null(state.GetState<string>("username"));
        Assert.Equal(0, state.GetState<int>("counter")); // Default value of int is 0
    }
}
