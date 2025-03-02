namespace SpfDemo.PromptHandlers.Services;

public interface INoteRepository
{
    Task CreateNoteAsync(string note);
    Task DeleteNoteAsync();
}

public class NoteRepository : INoteRepository
{
    public Task CreateNoteAsync(string note)
    {
        Console.WriteLine($"Note created (repository): {note}");
        return Task.CompletedTask;
    }

    public Task DeleteNoteAsync()
    {
        Console.WriteLine("Note deleted (repository)");
        return Task.CompletedTask;
    }
}
