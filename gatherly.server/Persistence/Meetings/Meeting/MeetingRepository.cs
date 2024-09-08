using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.Meeting;
using NHibernate;

namespace gatherly.server.Persistence.Meetings.Meeting;

public class MeetingRepository : IMeetingRepository
{
    private readonly ISessionFactory _sessionFactory;

    public MeetingRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task<Models.Meetings.Meeting.Meeting> CreateNewMeeting(Guid ownerId, MeetingDTOCreate meetingDto)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                // Correctly set the DateTimeKind
                var startOfTheMeeting = DateTime.SpecifyKind(meetingDto.StartOfTheMeeting, DateTimeKind.Unspecified);
                var endOfTheMeeting = DateTime.SpecifyKind(meetingDto.EndOfTheMeeting, DateTimeKind.Unspecified);

                // Retrieve the timezone
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(meetingDto.TimeZone);

                // Convert the DateTime to UTC using the specified timezone
                var startUtc = TimeZoneInfo.ConvertTimeToUtc(startOfTheMeeting, timezone);
                var endUtc = TimeZoneInfo.ConvertTimeToUtc(endOfTheMeeting, timezone);

                // Create the meeting entity
                var meeting = new Models.Meetings.Meeting.Meeting
                {
                    Id = Guid.NewGuid(),
                    OwnerId = ownerId,
                    MeetingName = meetingDto.MeetingName,
                    Description = meetingDto.Description,
                    PlaceName = meetingDto.PlaceName,
                    Lon = meetingDto.Lon,
                    Lat = meetingDto.Lat,
                    StartOfTheMeeting = startUtc,
                    EndOfTheMeeting = endUtc,
                    IsMeetingTimePlanned = meetingDto.IsMeetingTimePlanned,
                    TimeZone = timezone,
                    CreationTime = DateTime.UtcNow
                };

                // Save the meeting and commit the transaction
                await session.SaveAsync(meeting);
                await transaction.CommitAsync();

                return meeting;
            }
        }
    }


    public async Task<Models.Meetings.Meeting.Meeting> GetMeetingById(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var meeting = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);
            meeting.StartOfTheMeeting = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartOfTheMeeting, meeting.TimeZone);
            meeting.EndOfTheMeeting = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndOfTheMeeting, meeting.TimeZone);
            return meeting;
        }
    }

public async Task<Models.Meetings.Meeting.Meeting> UpdateAllMeetingData(Guid meetingId, Models.Meetings.Meeting.Meeting meetingNewData)
{
    using (var session = _sessionFactory.OpenSession())
    {
        using (var transaction = session.BeginTransaction())
        {
            var meeting = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);

            if (meeting == null) return null;
            
            meeting.MeetingName = meetingNewData.MeetingName;
            meeting.Description = meetingNewData.Description;
            meeting.PlaceName = meetingNewData.PlaceName;
            meeting.Lon = meetingNewData.Lon;
            meeting.Lat = meetingNewData.Lat;
            meeting.StartOfTheMeeting = meetingNewData.StartOfTheMeeting;
            meeting.EndOfTheMeeting = meetingNewData.EndOfTheMeeting;
            meeting.IsMeetingTimePlanned = meetingNewData.IsMeetingTimePlanned;
            meeting.TimeZone = meetingNewData.TimeZone;

            await session.UpdateAsync(meeting);
            await transaction.CommitAsync();
            return meeting;
        }
    }
}



    public async Task<bool> DeleteMeeting(Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var user = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);
                    if (user == null) return false;

                    await session.DeleteAsync(user);
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
    

    public async Task<Models.Meetings.Meeting.Meeting> ChangeMeetingPlaningMode(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var meeting = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);
                    if (meeting == null) return null;
                    meeting.IsMeetingTimePlanned = !meeting.IsMeetingTimePlanned;
                    await session.UpdateAsync(meeting);
                    await transaction.CommitAsync();
                    return meeting;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }

    public async Task<Models.Meetings.Meeting.Meeting> ChangeMeetingDateFrames(Guid meetingId, DateTime newStartDate,
        DateTime newEndDate)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var meeting = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);
                    if (meeting == null) return null;
                    meeting.IsMeetingTimePlanned = !meeting.IsMeetingTimePlanned;
                    meeting.StartOfTheMeeting = TimeZoneInfo.ConvertTimeToUtc(newStartDate,meeting.TimeZone);
                    meeting.EndOfTheMeeting = TimeZoneInfo.ConvertTimeToUtc(newEndDate, meeting.TimeZone);
                    await session.UpdateAsync(meeting);
                    await transaction.CommitAsync();
                    return meeting;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }

    public async Task<bool> IsUserAnMeetingOwner(Guid meetingId, Guid userId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var meeting = await session.GetAsync<Models.Meetings.Meeting.Meeting>(meetingId);
            if (meeting == null)
            {
                return false;
            }

            if (meeting.OwnerId != userId)
            {
                return false;
            }

            return true;
        }
    }
}