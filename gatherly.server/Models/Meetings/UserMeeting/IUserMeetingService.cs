using gatherly.server.Entities.Meetings;

namespace gatherly.server.Models.Meetings.UserMeeting;

public interface IUserMeetingService
{
    public Task<List<AvailabilityTimes>> PreparePosibleDateTimes(Guid meetingId); 
    public Task<List<UserEntityDTOMeetingInfo>> GetAllInvites(Guid meetingId); 
    public Task<List<UserEntityDTOMeetingInfo>> GetAllConfirmedInvites(Guid meetingId);
    public Task<List<UserEntityDTOMeetingInfo>> GetAllPendingInvites(Guid meetingId);
    public Task<List<UserEntityDTOMeetingInfo>> GetAllRejectedInvites(Guid meetingId);
    public Task<UserMeeting> CreateNewUserMeetingEntity(UserMeetingDTOCreate userMeetingDtoCreate);
    public Task<bool> DeleteUserMeetingEntity(Guid userMeetingId);
    public Task<bool> DeleteUserMeetingEntity(Guid userId, Guid meetingId);
    public Task<Guid?> GetUserMeetingId(Guid userId, Guid meetingId);
    public Task<InvitationStatus?> GetUserMeetingStatus(Guid userId, Guid meetingId);
    public Task<UserMeeting> ChangeInvitationStatus(Guid userMeetingId, InvitationStatus status);
    public Task<UserMeeting> ChangeAvailbilityTimes(Guid userMeetingId, byte[] times);
    public Task<UserMeeting> ChangeAvailbilityTimeFrames(Guid meetingId, int offset);
    public Task<int> CountAllMeetingsByUserId(Guid userId);
    public Task<List<MeetingDTOInfo>> GetAllMeetingsByUserId(Guid userId);
    public Task<UserMeeting> GetInviteByIds(Guid meetingId, Guid userId);
    public Task<bool> IsUserInMeeting(Guid userId, Guid meetingId);
    public Task<string?> GetUserTimes(Guid userId, Guid meetingId);
    public Task<List<AvailabilityTimesDTO>> GetAllUserTimes(Guid meetingId, Guid ownerId);

}