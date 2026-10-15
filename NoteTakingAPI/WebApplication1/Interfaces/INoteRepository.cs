using NoteTakingAPI.Models;
using System.Buffers;

namespace NoteTakingAPI.Interfaces
{
    public interface INoteRepository
    {
        public Task<NoteModel> GetNoteByTitle(int ownerId, string title);
        public Task<List<NoteModel>> GetAllNotesByOwner(int ownerId);
        public Task<List<NoteModel>> GetAllNotesByTeam(string teamName);
        public Task CreateOrUpdateNote(NoteModel note);
        public Task DeleteNote(int ownerId, string title);
    }
}
