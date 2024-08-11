using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.Meeting;

namespace gatherly.server.Persistence.Meetings.Meeting;

public class MeetingService : IMeetingService
{
    private readonly MeetingRepository _meetingRepository = new(NHibernateHelper.SessionFactory);
    
    public  Task<Models.Meetings.Meeting.Meeting> CreateNewMeeting(Guid ownerId, MeetingDTOCreate meeting)
    { 
        return _meetingRepository.CreateNewMeeting(ownerId, meeting);
    }

    public Task<Models.Meetings.Meeting.Meeting> GetMeetingById(Guid meetingId)
    {
        return _meetingRepository.GetMeetingById(meetingId);
    }

    public Task<Models.Meetings.Meeting.Meeting> UpdateAllMeetingData(Guid meetingId, Models.Meetings.Meeting.Meeting meeting)
    {
        return _meetingRepository.UpdateAllMeetingData(meetingId, meeting);
    }

    public Task<bool> DeleteMeeting(Guid meetingId)
    {
        return _meetingRepository.DeleteMeeting(meetingId);
    }

    public Task<Models.Meetings.Meeting.Meeting> ChangeMeetingPlaningMode(Guid meetingId)
    {
        return _meetingRepository.ChangeMeetingPlaningMode(meetingId);
    }

    public Task<Models.Meetings.Meeting.Meeting> ChangeMeetingDateFrames(Guid meetingId, DateTime newStartDate, DateTime newEndDate)
    {
        return _meetingRepository.ChangeMeetingDateFrames(meetingId, newStartDate, newEndDate);
    }

    public Task<bool> IsUserAnMeetingOwner(Guid meetingId, Guid userId)
    {
        return _meetingRepository.IsUserAnMeetingOwner(meetingId, userId);
    }
}