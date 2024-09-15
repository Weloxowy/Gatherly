using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Persistence.Authentication.UserEntity;

namespace gatherly.server.Persistence.Meetings.UserMeeting;

public class UserMeetingService : IUserMeetingService
{
    private readonly UserMeetingRepository _userMeetingRepository = new(NHibernateHelper.SessionFactory);
    
    public Task<List<AvailabilityTimes>> PreparePosibleDateTimes(Guid meetingId)
    {
        return _userMeetingRepository.PreparePosibleDateTimes(meetingId);
    }

    public Task<List<UserEntityDTOMeetingInfo>> GetAllInvites(Guid meetingId)
    {
        return _userMeetingRepository.GetAllInvites(meetingId);
    }

    public Task<List<UserEntityDTOMeetingInfo>> GetAllConfirmedInvites(Guid meetingId)
    {
        return _userMeetingRepository.GetAllConfirmedInvites(meetingId);
    }

    public Task<List<UserEntityDTOMeetingInfo>> GetAllPendingInvites(Guid meetingId)
    {
        return _userMeetingRepository.GetAllPendingInvites(meetingId);
    }

    public Task<List<UserEntityDTOMeetingInfo>> GetAllRejectedInvites(Guid meetingId)
    {
        return _userMeetingRepository.GetAllRejectedInvites(meetingId);
    }

    public Task<Models.Meetings.UserMeeting.UserMeeting> CreateNewUserMeetingEntity(UserMeetingDTOCreate userMeetingDtoCreate)
    {
        return _userMeetingRepository.CreateNewUserMeetingEntity(userMeetingDtoCreate);
    }

    public Task<bool> DeleteUserMeetingEntity(Guid userMeetingId)
    {
        return _userMeetingRepository.DeleteUserMeetingEntity(userMeetingId);
    }
    public Task<Guid?> GetUserMeetingId(Guid userId, Guid meetingId)
    {
        return _userMeetingRepository.GetUserMeetingId(userId, meetingId);
    }

    public Task<InvitationStatus?> GetUserMeetingStatus(Guid userId, Guid meetingId)
    {
        return _userMeetingRepository.GetUserMeetingStatus(userId, meetingId);
    }
    public Task<Models.Meetings.UserMeeting.UserMeeting> ChangeInvitationStatus(Guid userMeetingId, InvitationStatus status)
    {
        return _userMeetingRepository.ChangeInvitationStatus(userMeetingId, status);
    }

    public Task<Models.Meetings.UserMeeting.UserMeeting> ChangeAvailbilityTimes(Guid userMeetingId, byte[] times)
    {
        return _userMeetingRepository.ChangeAvailbilityTimes(userMeetingId, times);
    }

    public Task<Models.Meetings.UserMeeting.UserMeeting> ChangeAvailbilityTimeFrames(Guid meetingId, int offset)
    {
        return _userMeetingRepository.ChangeAvailbilityTimeFrames(meetingId, offset);
    }

    public Task<int> CountAllMeetingsByUserId(Guid userId)
    {
        return _userMeetingRepository.CountAllMeetingsByUserId(userId);
    }

    public Task<List<MeetingDTOInfo>> GetAllMeetingsByUserId(Guid userId)
    {
        return _userMeetingRepository.GetAllMeetingsByUserId(userId);
    }

    public Task<Models.Meetings.UserMeeting.UserMeeting> GetInviteByIds(Guid meetingId, Guid userId)
    {
        return _userMeetingRepository.GetInviteByIds(meetingId, userId);
    }

    public Task<bool> IsUserInMeeting(Guid userId, Guid meetingId)
    {
        return _userMeetingRepository.IsUserInMeeting(userId, meetingId);
    }

}