using SpfDemo.PromptHandlers.Services;
using SpfFramework;

namespace SpfDemo.PromptHandlers.Note;

public class Delete(INoteRepository noteRepository) : ISpfPromptHandler
{
    public async Task HandlePromptAsync(string[] path, string[] input, SpfState state)
    {
        await noteRepository.DeleteNoteAsync();
    }
}
