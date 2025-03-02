using SimplePromptFramework;
using SimplePromptFramework.Demo.PromptHandlers.Services;

namespace SimplePromptFramework.Demo.PromptHandlers.Note;

public class Delete(INoteRepository noteRepository) : ISpfPromptHandler
{
    public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
    {
        await noteRepository.DeleteNoteAsync();
    }
}
