using gatherly.server.Models.Meetings.UserMeeting;

namespace gatherly.server.Entities.Meetings;

public class UserEntityDTOMeetingInfo
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }

    public virtual string Email { get; set; }
    public virtual string Name { get; set; }
    public virtual string Avatar { get; set; }
    public virtual InvitationStatus Status { get; set; }
}