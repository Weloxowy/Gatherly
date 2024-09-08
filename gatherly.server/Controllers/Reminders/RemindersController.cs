using System.Text;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace gatherly.server.Controllers.Reminders;

[ApiController]
[Route("/api/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly IUserMeetingService _userMeetingService;
    private readonly IInvitationsService _invitationsService;
    private readonly IUserEntityService _userService;
    private readonly ITokenEntityService _tokenService;
    private readonly IMailEntityService _mailService;
    
    public RemindersController(IMeetingService meetingService, IUserMeetingService userMeetingService,
        IInvitationsService invitationsService, IUserEntityService userService, ITokenEntityService tokenService,
        IMailEntityService mailService)
    {
        _meetingService = meetingService;
        _userMeetingService = userMeetingService;
        _invitationsService = invitationsService;
        _userService = userService;
        _tokenService = tokenService;
        _mailService = mailService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetRemindersScope()
    {
        // Pobierz ID użytkownika z tokenu
        var id = _tokenService.GetIdFromRequestCookie(HttpContext);

        // Ścieżka do folderu z reminders
        var remindersPath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "reminders");

        // Sprawdź, czy folder istnieje
        if (!Directory.Exists(remindersPath))
        {
            return NotFound("Folder reminders nie istnieje.");
        }

        // Ścieżka do pliku z notatką
        var filePath = Path.Combine(remindersPath, $"{id}.json");

        // Sprawdź, czy plik istnieje
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                // Odczytaj zawartość pliku
                var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                return Ok(jsonContent);
            }
            catch (Exception ex)
            {
                // Obsłuż wyjątek, jeśli coś poszło nie tak podczas odczytu
                return StatusCode(500, $"Błąd podczas odczytywania pliku: {ex.Message}");
            }
        }

        // Jeśli plik nie istnieje, utwórz go
        try
        {
            using (var stream = System.IO.File.Create(filePath))
            {
                byte[] initialContent = Encoding.UTF8.GetBytes("{\n  \"notes\": {\n    \"note\": [\n    ]\n  }\n}");
                await stream.WriteAsync(initialContent, 0, initialContent.Length);
                var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                return Ok(jsonContent);
            }

        }
        catch (Exception ex)
        {
            // Obsłuż wyjątek, jeśli coś poszło nie tak podczas tworzenia
            return StatusCode(500, $"Błąd podczas tworzenia pliku: {ex.Message}");
        }
    }
    
public class Note
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool? Checked { get; set; } // Opcjonalnie
}

public class Notes
{
    public List<Note> Note { get; set; }
}

public class ReminderData
{
    public Notes Notes { get; set; }
}

[HttpPut]
[Authorize]
public async Task<ActionResult> PutRemindersScope([FromBody] ReminderData data)
{
    var id = _tokenService.GetIdFromRequestCookie(HttpContext);
    var remindersPath = Path.Combine(Directory.GetCurrentDirectory(), "Content", "reminders");

    if (!Directory.Exists(remindersPath))
    {
        return NotFound("Folder reminders nie istnieje.");
    }

    var filePath = Path.Combine(remindersPath, $"{id}.json");

    try
    {
        // Walidacja na poziomie obiektu
        if (data == null || data.Notes == null || data.Notes.Note == null)
        {
            return BadRequest("Dane JSON są nieprawidłowe: Brak wymaganych pól.");
        }

        foreach (var note in data.Notes.Note)
        {
            if (string.IsNullOrEmpty(note.Id) || string.IsNullOrEmpty(note.Text))
            {
                return BadRequest($"Dane JSON są nieprawidłowe: Brak wymaganego pola w notatce o ID {note.Id}.");
            }
        }

        // Zapisz dane do pliku
        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        await System.IO.File.WriteAllTextAsync(filePath, jsonString);
        return Ok("Dane zostały zapisane.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Błąd podczas operacji: {ex.Message}");
    }
}

}