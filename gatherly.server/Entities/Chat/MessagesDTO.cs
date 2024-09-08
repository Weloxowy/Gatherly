using gatherly.server.Models.Chat.Chat;

namespace gatherly.server.Entities.Chat;

public class MessagesDTO
{
    public virtual int Id { get; set; }
    public virtual string Content { get; set; }
    public virtual DateTime Timestamp { get; set; }
    public virtual Guid SenderId { get; set; }
    public virtual Guid MeetingId { get; set; }
    public virtual TypesOfMessage TypesOfMessage { get; set; } 
    public virtual string UserName { get; set; }
    public virtual string UserAvatar { get; set; }
}
/*
public static class MessagesDTOMapping
{
    public static MessagesDTO ToDto(this Message message)
    {
        return new MessagesDTO(
            message.Id,
            message.Content,
            message.Timestamp,
            message.SenderId,
            message.MeetingId,
            message.senderId == 'ff' ? TypesOfMessage.System : TypesOfMessage.Me
        );
    }
}
*/