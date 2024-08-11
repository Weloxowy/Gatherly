namespace gatherly.server.Entities.Meetings
{
    public class MeetingDTOInfo
    {
        public virtual Guid Id { get; set; }
        public virtual string MeetingName { get; set; }
        public virtual string Description { get; set; }
        public virtual string PlaceName { get; set; }
        public virtual DateTime StartOfTheMeeting { get; set; }
        public virtual DateTime EndOfTheMeeting { get; set; }
        public virtual TimeZoneInfo TimeZone { get; set; }
    }
}
