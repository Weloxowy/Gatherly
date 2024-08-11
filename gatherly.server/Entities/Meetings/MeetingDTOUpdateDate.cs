namespace gatherly.server.Entities.Meetings;

public class MeetingDTOUpdateDate
{
    public virtual Guid Id { get; set; }
    public virtual DateTime StartOfTheMeeting { get; set; }
    public virtual DateTime EndOfTheMeeting { get; set; }
}