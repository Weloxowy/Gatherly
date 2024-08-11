namespace gatherly.server.Models.Meetings.Invitations;

public class Invitations
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual Guid MeetingId { get; set; }
    public virtual DateTime ValidTime { get; set; }

}