using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Entities.Meetings;

public class FullMeetingDTOInfo
{
    public virtual Guid Id { get; set; }
    public virtual Guid OwnerId { get; set; }
    public virtual string OwnerName { get; set; }
    public virtual string MeetingName { get; set; }
    public virtual string Description { get; set; }
    public virtual string PlaceName { get; set; }
    public virtual double? Lon { get; set; }
    public virtual double? Lat { get; set; }
    public virtual DateTime StartOfTheMeeting { get; set; }
    public virtual DateTime EndOfTheMeeting { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual bool IsMeetingTimePlanned { get; set; }
    public virtual TimeZoneInfo TimeZone { get; set; }
    public virtual bool isRequestingUserAnOwner { get; set; }
}