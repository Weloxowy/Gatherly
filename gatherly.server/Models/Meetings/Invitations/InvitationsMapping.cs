using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Meetings.Invitations;

public class InvitationsMapping : ClassMap<Invitations>
{
    public InvitationsMapping()
    {
        Table("Invitations");
        Id(x => x.Id);
        Map(x => x.UserId);
        Map(x => x.MeetingId);
        Map(x => x.ValidTime);
    }
}