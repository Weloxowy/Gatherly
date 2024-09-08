using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Chat.Chat;

public class MessageMapping: ClassMap<Message>
{
    public MessageMapping()
    {
        Table("Message");
        Id(x => x.Id);
        Map(x => x.MeetingId);
        Map(x => x.Content);
        Map(x => x.Timestamp);
        Map(x => x.SenderId);
    }
}