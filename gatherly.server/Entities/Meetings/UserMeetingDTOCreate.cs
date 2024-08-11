using gatherly.server.Models.Meetings.UserMeeting;

namespace gatherly.server.Entities.Meetings;

public class UserMeetingDTOCreate
{
    public virtual Guid MeetingId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual InvitationStatus Status { get; set; }
    public virtual byte[] Availability { get; set; }
}