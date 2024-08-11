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
                var meeting = new Models.Meetings.Meeting.Meeting
                {
                    Id = Guid.NewGuid(),
                    OwnerId = ownerId,
                    MeetingName = meetingDto.MeetingName,
                    Description = meetingDto.Description,
                    PlaceName = meetingDto.PlaceName,
                    Lon = meetingDto.Lon,
                    Lat = meetingDto.Lat,
                    StartOfTheMeeting = meetingDto.StartOfTheMeeting,
                    EndOfTheMeeting = meetingDto.EndOfTheMeeting,
                    IsMeetingTimePlanned = meetingDto.IsMeetingTimePlanned,
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById(meetingDto.TimeZone),
                };

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

                var meetingType = typeof(Models.Meetings.Meeting.Meeting);
                var newDataType = meetingNewData.GetType();
                var meetingProperties = meetingType.GetProperties();

                foreach (var property in meetingProperties)
                {
                    var newDataProperty = newDataType.GetProperty(property.Name);
                    if (newDataProperty != null && newDataProperty.PropertyType == property.PropertyType)
                    {
                        var value = newDataProperty.GetValue(meetingNewData);
                        if (value != null) property.SetValue(meeting, value);
                    }
                }

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
                    meeting.StartOfTheMeeting = newStartDate;
                    meeting.EndOfTheMeeting = newEndDate;
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