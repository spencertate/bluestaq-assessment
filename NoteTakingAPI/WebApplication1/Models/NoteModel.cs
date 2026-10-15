namespace NoteTakingAPI.Models
{
    public class NoteModel
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required int OwnerId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }
}
