namespace gatherly.server.Models.Meetings.Meeting;

public class Meeting
{

    public virtual Guid Id { get; set; }
    public virtual Guid OwnerId { get; set; }
    public virtual string MeetingName { get; set; }
    public virtual string Description { get; set; }
    public virtual string PlaceName { get; set; }
    public virtual double? Lon { get; set; }
    public virtual double? Lat { get; set; }
    public virtual DateTime StartOfTheMeeting { get; set; } //max 7 dni
    public virtual DateTime EndOfTheMeeting { get; set; }
    public virtual bool IsMeetingTimePlanned { get; set; } //TRUE - uruchomiony jest moduł dopasowywania terminów; wtedy start-end to są przedziały do wybrania
    public virtual TimeZoneInfo TimeZone { get; set; }
    
}