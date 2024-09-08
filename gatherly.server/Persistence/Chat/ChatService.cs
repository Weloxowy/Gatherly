using gatherly.server;
using gatherly.server.Entities.Chat;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Persistence;
using gatherly.server.Persistence.Chat;
using NHibernate;

public class ChatService : IChatService
{
    private readonly ChatRepository _chatRepository = new(NHibernateHelper.SessionFactory);
    
    public async Task SaveMessageAsync(Message message)
    {
       await _chatRepository.SaveMessageAsync(message);
    }
    public async Task<List<MessagesDTO>> GetLastMessagesAsync(Guid meetingId, Guid userId)
    {
        return await _chatRepository.GetLastMessagesAsync(meetingId, userId);
    }

    public async Task<List<Message>> GetMessagesFromDateAsync(Guid meetingId, DateTime startDate, int count)
    {
        return await _chatRepository.GetMessagesFromDateAsync(meetingId, startDate, count);
    }

}