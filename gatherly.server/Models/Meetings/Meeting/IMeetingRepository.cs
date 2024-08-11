using gatherly.server.Entities.Meetings;

namespace gatherly.server.Models.Meetings.Meeting;

public interface IMeetingRepository
{
    public Task<Meeting> CreateNewMeeting(Guid ownerId, MeetingDTOCreate meeting);
    public Task<Meeting> GetMeetingById(Guid meetingId);
    public Task<Meeting> UpdateAllMeetingData(Guid meetingId, Meeting meeting);
    //update pare wariantow - zmienne jest DTO : date,place,name,...
    public Task<bool> DeleteMeeting(Guid meetingId);
    public Task<Meeting> ChangeMeetingPlaningMode(Guid meetingId);
    public Task<Meeting> ChangeMeetingDateFrames(Guid meetingId, DateTime newStartDate, DateTime newEndDate);
    public Task<bool> IsUserAnMeetingOwner(Guid meetingId, Guid userId);

}