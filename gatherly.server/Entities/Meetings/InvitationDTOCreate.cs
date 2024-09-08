namespace gatherly.server.Entities.Meetings;

public class InvitationDTOCreate
{
    public virtual string UserEmail { get; set; }
    public virtual Guid MeetingId { get; set; }
}