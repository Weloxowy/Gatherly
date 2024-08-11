namespace gatherly.server.Entities.Meetings;

public class InvitationDTOCreate
{
    public virtual Guid UserId { get; set; }
    public virtual Guid MeetingId { get; set; }
}