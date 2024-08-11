namespace gatherly.server.Models.Meetings.UserMeeting;

public class UserMeeting
{
    public virtual Guid Id { get; set; }
    public virtual Guid MeetingId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual InvitationStatus Status { get; set; }
    public virtual byte[] Availability { get; set; }
    
}