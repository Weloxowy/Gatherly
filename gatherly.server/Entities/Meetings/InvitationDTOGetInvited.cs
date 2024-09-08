namespace gatherly.server.Entities.Meetings;

public class InvitationDTOGetInvited
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string UserName { get; set; }
    public virtual string UserAvatar { get; set; }
    public virtual Guid MeetingId { get; set; }
    public virtual DateTime ValidTime { get; set; }
}