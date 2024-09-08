namespace gatherly.server.Models.Chat.Chat;

public class Message
{
    public Message() : base()
    {
    }

    public Message(int id, string content, DateTime timestamp, Guid senderId, Guid meetingId)
    {
        Id = id;
        Content = content;
        Timestamp = timestamp;
        SenderId = senderId;
        MeetingId = meetingId;
    }

    public virtual int Id { get; set; }
    public virtual string Content { get; set; }
    public virtual DateTime Timestamp { get; set; }
    public virtual Guid SenderId { get; set; }
    public virtual Guid MeetingId { get; set; }
    
}