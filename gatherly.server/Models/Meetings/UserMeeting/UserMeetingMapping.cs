using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Meetings.UserMeeting;

public class UserMeetingMapping : ClassMap<UserMeeting>
{
    public UserMeetingMapping()
    {
        Table("UserMeeting");
        Id(x => x.Id);
        Map(x => x.MeetingId);
        Map(x => x.UserId);
        Map(x => x.Status).CustomType<InvitationStatus>();
        Map(x => x.Availability);
    }
}