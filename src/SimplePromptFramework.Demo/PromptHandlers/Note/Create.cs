using SimplePromptFramework.Demo.PromptHandlers.Services;

namespace SimplePromptFramework.Demo.PromptHandlers.Note;

public class Create(INoteRepository noteRepository) : ISpfPromptHandler
{
    public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
    {
        await noteRepository.CreateNoteAsync(string.Join(" ", input));
    }
}
