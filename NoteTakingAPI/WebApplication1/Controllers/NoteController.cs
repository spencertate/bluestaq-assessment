using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NoteTakingAPI.Interfaces;
using NoteTakingAPI.Models;

namespace NoteTakingAPI.Controllers
{
    [ApiController]
    [Route("note")]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly INoteRepository _databaseRepository;
        public NoteController(ILogger<NoteController> logger, INoteRepository databaseRepository)
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
        }

        [HttpGet]
        public ActionResult<NoteModel> GetNoteByTitle([FromBody] string title)
        {
            HttpContext.Request.Headers.TryGetValue("OwnerId", out var ownerIdHeader);
            int ownerId = int.TryParse(ownerIdHeader, out var parsedOwnerId) ? parsedOwnerId : 0;
            if (parsedOwnerId == 0)
            {
                _logger.LogWarning("OwnerId header is missing or invalid.");
                return BadRequest("OwnerId header is required and must be a valid integer.");
            }
            NoteModel? note = _databaseRepository.GetNoteByTitle(ownerId, title).Result;
            if (note == null) {
                _logger.LogWarning($"Note with Title: {title} not found.");
                return NotFound($"Note with Title: {title} not found.");
            }
            return Ok(note);
        }

        [HttpGet("all")]
        public ActionResult<List<NoteModel>> GetAllNotesByOwner()
        {
            HttpContext.Request.Headers.TryGetValue("OwnerId", out var ownerIdHeader);
            int ownerId = int.TryParse(ownerIdHeader, out var parsedOwnerId) ? parsedOwnerId : 0;
            if (parsedOwnerId == 0)
            {
                _logger.LogWarning("OwnerId header is missing or invalid.");
                return BadRequest("OwnerId header is required and must be a valid integer.");
            }
            _logger.LogInformation($"Getting all notes for owner with ID: {ownerId}");
            List<NoteModel> notes = _databaseRepository.GetAllNotesByOwner(ownerId).Result;
            return Ok(notes);
        }

        [HttpGet("team")]
        public ActionResult<List<NoteModel>> GetAllNotesByTeam()
        {
            HttpContext.Request.Headers.TryGetValue("TeamName", out var teamNameHeader);
            if (string.IsNullOrEmpty(teamNameHeader))
            {
                // If the team name is missing, return a bad request. 
                // It is possible for notes to be created without a team name,
                // but we don't want to return all notes with an empty team name, because that is not a valid team.
                _logger.LogWarning("TeamName header is missing.");
                return BadRequest("TeamName is missing from the header.");
            }
            _logger.LogInformation($"Getting all notes for team: {teamNameHeader}");
            return Ok(_databaseRepository.GetAllNotesByTeam(teamNameHeader).Result);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateNote([FromBody] CreateNoteRequest request)
        {
            _logger.LogInformation($"Creating or updating note with ID: {request.Title}");
            HttpContext.Request.Headers.TryGetValue("OwnerId", out var ownerIdHeader);
            int ownerId = int.TryParse(ownerIdHeader, out var parsedOwnerId) ? parsedOwnerId : 0;
            if (ownerId == 0)
            {
                _logger.LogWarning("OwnerId header is missing or invalid.");
                return BadRequest("OwnerId header is required and must be a valid integer.");
            }
            if (!HttpContext.Request.Headers.TryGetValue("TeamName", out var teamNameHeader))
            {
                teamNameHeader = string.Empty;
            }

            NoteModel noteModel = new NoteModel
            {
                Title = request.Title,
                Content = request.Content,
                OwnerId = ownerId,
                TeamName = teamNameHeader
            };

            _databaseRepository.CreateOrUpdateNote(noteModel);
            return Created();
        }

        [HttpDelete]
        public ActionResult DeleteNote([FromBody] string title)
        {
            HttpContext.Request.Headers.TryGetValue("OwnerId", out var ownerIdHeader);
            int ownerId = int.TryParse(ownerIdHeader, out var parsedOwnerId) ? parsedOwnerId : 0;
            if (parsedOwnerId == 0)
            {
                _logger.LogWarning("OwnerId header is missing or invalid.");
                return BadRequest("OwnerId header is required and must be a valid integer.");
            }
            _logger.LogInformation($"Deleting note with ID: {title} for owner with ID: {ownerId}");
            _databaseRepository.DeleteNote(ownerId, title);
            return NoContent();
        }
    }
}
