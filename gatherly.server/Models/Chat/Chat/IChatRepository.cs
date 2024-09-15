using gatherly.server.Entities.Chat;

namespace gatherly.server.Models.Chat.Chat;

public interface IChatRepository
{
    public Task SaveMessageAsync(Message message);
    public Task<List<MessagesDTO>> GetLastMessagesAsync(Guid meetingId, Guid userId);
    public Task<List<Message>> GetMessagesFromDateAsync(Guid meetingId, DateTime startDate, int count);
    public Task SaveSystemMessageAsync(Guid meetingId, string text);

}