namespace gatherly.server.Entities.Meetings;

public class MeetingDTOUpdateBasic
{
    public virtual Guid Id { get; set; }
    public virtual string MeetingName { get; set; }
    public virtual string Description { get; set; }
    public virtual string TimeZone { get; set; }
}