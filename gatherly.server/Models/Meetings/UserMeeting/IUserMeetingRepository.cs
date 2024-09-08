using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Models.Meetings.UserMeeting;

public interface IUserMeetingRepository
{
    public Task<List<AvailabilityTimes>> PreparePosibleDateTimes(Guid meetingId); //wyszukanie pasujacych godzin
    public Task<List<UserEntityDTOMeetingInfo>> GetAllInvites(Guid meetingId); //zebranie wszystkich osób na spotkanie
    public Task<List<UserEntityDTOMeetingInfo>> GetAllConfirmedInvites(Guid meetingId); //zbiór wszystkich tych co potwierdzili przybycie
    public Task<List<UserEntityDTOMeetingInfo>> GetAllPendingInvites(Guid meetingId);  //zbiór wszystkich tych co nie potwierdzili przybycia
    public Task<List<UserEntityDTOMeetingInfo>> GetAllRejectedInvites(Guid meetingId); //zbiór wszystkich tych co odrzucili przybycie
    
    public Task<UserMeeting> CreateNewUserMeetingEntity(UserMeetingDTOCreate userMeetingDtoCreate); //stworzenie nowej encji - jak ktoś zaakceptuje zaproszenie
    public Task<bool> DeleteUserMeetingEntity(Guid userMeetingId); //usunięcie encji - jak ktoś opuści spotkanie
    public Task<UserMeeting> ChangeInvitationStatus(Guid userMeetingId, InvitationStatus status); //zmiana statusu przybycia
    public Task<UserMeeting> ChangeAvailbilityTimes(Guid userMeetingId, byte[] times); //zmiana czasu jaki komuś pasuje na spotkanie
    public Task<UserMeeting> ChangeAvailbilityTimeFrames(Guid meetingId, int offset); //jak ktos zmieni ramy spotkania to przesuwamy bity żeby zostały tylko te które są w danym spotkaniu
    //np. jak ktoś przesunie ramy spotkania o 3 dni do przodu to bity z 3 pierwszych dni zostaną usunięte
    public Task<int> CountAllMeetingsByUserId(Guid userId); //ilosc spotkan danego uzytkownika - spotkan których data jest później niz aktualna
    public Task<List<MeetingDTOInfo>> GetAllMeetingsByUserId(Guid userId); //lista spotkan danego uzytkownika ktore sie nie odbyly
    public Task<UserMeeting> GetInviteByIds(Guid meetingId, Guid userId); //znajdz encje usermeeting po id spotkania i usera


}