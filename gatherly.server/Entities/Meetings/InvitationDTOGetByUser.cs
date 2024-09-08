namespace gatherly.server.Entities.Meetings;

public class InvitationDTOGetByUser
{
    public virtual Guid InvitationId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual Guid MeetingId { get; set; }
    public virtual DateTime ValidTime { get; set; }
    public virtual Guid OwnerId { get; set; }
    public virtual string MeetingName { get; set; }
    public virtual string Description { get; set; }
    public virtual string PlaceName { get; set; }
    public virtual double? Lon { get; set; }
    public virtual double? Lat { get; set; }
    public virtual DateTime StartOfTheMeeting { get; set; } //max 7 dni
    public virtual DateTime EndOfTheMeeting { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual bool IsMeetingTimePlanned { get; set; } //TRUE - uruchomiony jest moduł dopasowywania terminów; wtedy start-end to są przedziały do wybrania
    public virtual TimeZoneInfo TimeZone { get; set; }
}