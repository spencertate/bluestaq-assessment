using NoteTakingAPI.Interfaces;
using NoteTakingAPI.Models;

namespace NoteTakingAPI.Services
{
    public class DictionaryNoteRepository : INoteRepository
    {
        public readonly Dictionary<string, NoteModel> _notes;

        public DictionaryNoteRepository()
        {
            _notes = new Dictionary<string, NoteModel>();
        }

        public Task CreateOrUpdateNote(NoteModel note)
        {
            foreach (var existingNote in _notes.Values)
            {
                if (existingNote.OwnerId == note.OwnerId && existingNote.Title == note.Title)
                {
                    // Update the existing note
                    existingNote.Content = note.Content;
                    existingNote.TeamName = note.TeamName;
                    return Task.CompletedTask;
                }
            }
            _notes.Add((GenerateKey(note.OwnerId, note.Title)), note);
            return Task.CompletedTask;
        }

        public Task DeleteNote(int ownerId, string title)
        {
            _notes.Remove(GenerateKey(ownerId, title));
            return Task.CompletedTask;
        }

        public Task<List<NoteModel>> GetAllNotesByOwner(int ownerId)
        {
            List<NoteModel> notesByOwner = new List<NoteModel>();
            foreach (var note in _notes.Values)
            {
                if (note.OwnerId == ownerId)
                {
                    notesByOwner.Add(note);
                }
            }
            return Task.FromResult<List<NoteModel>>(notesByOwner);
        }

        public Task<List<NoteModel>> GetAllNotesByTeam(string teamName)
        {
            List<NoteModel> notesByTeam = new List<NoteModel>();
            foreach (var note in _notes.Values)
            {
                if (note.TeamName == teamName)
                {
                    notesByTeam.Add(note);
                }
            }
            return Task.FromResult<List<NoteModel>>(notesByTeam);
        }

        public Task<NoteModel> GetNoteByTitle(int ownerId, string title)
        {
            _notes.TryGetValue(GenerateKey(ownerId, title), out NoteModel? note);
            return Task.FromResult<NoteModel>(note); // null is handled upstream
        }

        // We are using this as the key, it would have the same cardinality as DynamoDB would with owner as a primary key and title as the secondary key.
        private static string GenerateKey(int ownerId, string title)
        {
            return $"{ownerId}:{title}";
        }
    }
}
