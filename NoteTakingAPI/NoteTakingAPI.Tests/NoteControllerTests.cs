using Microsoft.AspNetCore.Mvc;
using NoteTakingAPI.Controllers;
using NoteTakingAPI.Models;
using NoteTakingAPI.Services;
using Xunit;

namespace NoteTakingAPI.Tests
{
    public class NoteControllerTests
    {
        private readonly NoteController _noteController;
        private readonly DictionaryNoteRepository _dictionaryNoteRepository;
        public NoteControllerTests()
        {
            _dictionaryNoteRepository = new DictionaryNoteRepository();
            _noteController = new NoteController(new LoggerFactory().CreateLogger<NoteController>(), _dictionaryNoteRepository);
            _noteController.ControllerContext.HttpContext = new DefaultHttpContext();
            _noteController.ControllerContext.HttpContext.Request.Headers["OwnerId"] = "1";
            _noteController.ControllerContext.HttpContext.Request.Headers["TeamName"] = "TeamA";
            _noteController.CreateOrUpdateNote(new CreateNoteRequest { Title = "First Note", Content = "This is my first note" });
            _noteController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public void GetNoteByTitle_ReturnsNote_WhenNoteExists()
        {
            // Arrange
            var title = "First Note";
            var ownerId = 1;
            _noteController.ControllerContext.HttpContext = new DefaultHttpContext();
            _noteController.ControllerContext.HttpContext.Request.Headers["OwnerId"] = ownerId.ToString();
            // Act
            var result = _noteController.GetNoteByTitle(title);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var note = Assert.IsType<NoteModel>(okResult.Value);
            Assert.NotNull(result);
            Assert.Equal(title, note.Title);
        }
    }
}
