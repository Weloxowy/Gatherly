using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Meetings.Meeting;

public class MeetingMapping : ClassMap<Meeting>
{
    public MeetingMapping()
    {
        Table("Meeting");
        Id(x => x.Id);
        Map(x => x.OwnerId);
        Map(x => x.MeetingName);
        Map(x => x.Description);
        Map(x => x.PlaceName);
        Map(x => x.Lon);
        Map(x => x.Lat);
        Map(x => x.StartOfTheMeeting);
        Map(x => x.EndOfTheMeeting);
        Map(x => x.IsMeetingTimePlanned);
        Map(x => x.TimeZone).CustomType<TimeZoneType>();
    }
}