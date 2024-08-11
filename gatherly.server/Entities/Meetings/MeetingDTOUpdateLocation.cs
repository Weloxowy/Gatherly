namespace gatherly.server.Entities.Meetings;

public class MeetingDTOUpdateLocation
{
    public virtual Guid Id { get; set; }
    public virtual string PlaceName { get; set; }
    public virtual double? Lon { get; set; }
    public virtual double? Lat { get; set; }
}