using System.Text;
using FluentEmail.Core;
using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.UserMeeting;
using NHibernate;
using NHibernate.Linq;

namespace gatherly.server.Persistence.Meetings.UserMeeting;

public class UserMeetingRepository : IUserMeetingRepository
{
    private readonly ISessionFactory _sessionFactory;

    public UserMeetingRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }
    
    public async Task<List<AvailabilityTimes>> PreparePosibleDateTimes(Guid meetingId)
{
    using (var session = _sessionFactory.OpenSession())
    {
        var userList = session.Query<Models.Meetings.UserMeeting.UserMeeting>()
            .Where(x => x.MeetingId == meetingId)
            .ToList();
        var meeting = session.Get<Models.Meetings.Meeting.Meeting>(meetingId);

        int totalSlots = (int)(meeting.EndOfTheMeeting - meeting.StartOfTheMeeting).TotalMinutes / 15;
        var availabilityCounts = new int[totalSlots];
        foreach (var user in userList)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                if (user.Availability.Length < totalSlots)
                {
                    break;
                }
                if ((user.Availability[byteIndex] & (1 << bitIndex)) != 0)
                {
                    availabilityCounts[i]++;
                }
            }
        }

        var commonAvailabilityList = new List<AvailabilityTimes>();
        for (int i = 0; i < totalSlots;)
        {
            if (availabilityCounts[i] > 1)
            {
                int start = i;
                int maxPeople = availabilityCounts[i];

                while (i < totalSlots && availabilityCounts[i] > 1)
                {
                    maxPeople = Math.Max(maxPeople, availabilityCounts[i]);
                    i++;
                }

                int end = i;

                var availabilityTimes = new AvailabilityTimes
                {
                    StartTime = meeting.StartOfTheMeeting.AddMinutes(start * 15),
                    EndTime = meeting.StartOfTheMeeting.AddMinutes(end * 15),
                    NumberOfPeople = maxPeople
                };
                commonAvailabilityList.Add(availabilityTimes);
            }
            else
            {
                i++;
            }
        }

        // Sort by number of users and duration, then take the top 5
        var topAvailabilities = commonAvailabilityList
            .OrderByDescending(a => a.NumberOfPeople)
            .ThenByDescending(a => a.EndTime - a.StartTime)
            .Take(5)
            .ToList();

        return topAvailabilities;
    }
}


    public async Task<List<UserEntityDTOMeetingInfo>> GetAllInvites(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId)
                .ToListAsync();

            List<UserEntityDTOMeetingInfo> users = new List<UserEntityDTOMeetingInfo>();
            foreach (var user in userList)
            {
                var userInfo = await session.GetAsync<Models.Authentication.UserEntity.UserEntity>(user.UserId);
                users.Add(
                    new UserEntityDTOMeetingInfo
                    {
                        Id = user.Id,
                        UserId = user.UserId,
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        Avatar = userInfo.AvatarName,
                        Status = user.Status
                    });
            }

            return users;
        }
    }
    
    public async Task<Models.Meetings.UserMeeting.UserMeeting> GetInviteByIds(Guid meetingId, Guid userId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            return await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }

    public async Task<int> CountAllMeetingsByUserId(Guid userId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var today = DateTime.UtcNow.Date;

            var meetingsList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.UserId == userId)
                .Join(session.Query<Models.Meetings.Meeting.Meeting>(),
                    userSession => userSession.MeetingId,
                    meetingSession => meetingSession.Id,
                    (userSession, meetingSession) => new { UserSession = userSession, MeetingSession = meetingSession })
                .Where(joined => joined.MeetingSession.StartOfTheMeeting.Date == today)
                .CountAsync();

            return meetingsList;
        }
    }

    public async Task<List<MeetingDTOInfo>> GetAllMeetingsByUserId(Guid userId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var today = DateTime.UtcNow.Date;

            var meetingsList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.UserId == userId)
                .Join(
                    session.Query<Models.Meetings.Meeting.Meeting>(),   // Druga tabela
                    userMeeting => userMeeting.MeetingId,               // Klucz obcy w UserMeeting
                    meeting => meeting.Id,                              // Klucz główny w Meeting
                    (userMeeting, meeting) => new { UserMeeting = userMeeting, Meeting = meeting } // Nowy anonimowy typ
                )
                // Filtrujemy na podstawie daty
                .Select(x => x.Meeting)                                // Wybieramy tylko spotkania
                .ToListAsync();

            var meetingDTOList = meetingsList.Select(meeting => new MeetingDTOInfo
            {
                Id = meeting.Id,
                MeetingName = meeting.MeetingName,
                Description = meeting.Description,
                PlaceName = meeting.PlaceName,
                StartOfTheMeeting = meeting.StartOfTheMeeting,
                EndOfTheMeeting = meeting.EndOfTheMeeting,
                TimeZone = meeting.TimeZone
            }).ToList();

            return meetingDTOList;
        }
    }


    public async Task<List<UserEntityDTOMeetingInfo>> GetAllConfirmedInvites(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.Status == InvitationStatus.Accepted)
                .ToListAsync();

            List<UserEntityDTOMeetingInfo> users = new List<UserEntityDTOMeetingInfo>();
            foreach (var user in userList)
            {
                var userInfo = await session.GetAsync<Models.Authentication.UserEntity.UserEntity>(user.UserId);
                users.Add(
                    new UserEntityDTOMeetingInfo
                    {
                        Id = user.Id,
                        UserId = user.UserId,
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        Avatar = userInfo.AvatarName,
                        Status = user.Status
                    });
            }

            return users;
        }
    }

    public async Task<List<UserEntityDTOMeetingInfo>> GetAllPendingInvites(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.Status == InvitationStatus.Pending)
                .ToListAsync();

            List<UserEntityDTOMeetingInfo> users = new List<UserEntityDTOMeetingInfo>();
            foreach (var user in userList)
            {
                var userInfo = await session.GetAsync<Models.Authentication.UserEntity.UserEntity>(user.UserId);
                users.Add(
                    new UserEntityDTOMeetingInfo
                    {
                        Id = user.Id,
                        UserId = user.UserId,
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        Avatar = userInfo.AvatarName,
                        Status = user.Status
                    });
            }

            return users;
        }
    }

    public async Task<List<UserEntityDTOMeetingInfo>> GetAllRejectedInvites(Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userList = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.Status == InvitationStatus.Rejected)
                .ToListAsync();

            List<UserEntityDTOMeetingInfo> users = new List<UserEntityDTOMeetingInfo>();
            foreach (var user in userList)
            {
                var userInfo = await session.GetAsync<Models.Authentication.UserEntity.UserEntity>(user.UserId);
                users.Add(
                    new UserEntityDTOMeetingInfo
                    {
                        Id = user.Id,
                        UserId = user.UserId,
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        Avatar = userInfo.AvatarName,
                        Status = user.Status
                    });
            }

            return users;
        }
    }

    public async Task<Models.Meetings.UserMeeting.UserMeeting> CreateNewUserMeetingEntity(UserMeetingDTOCreate userMeetingDtoCreate)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var meeting = session.Get<Models.Meetings.Meeting.Meeting>(userMeetingDtoCreate.MeetingId);
                var totalHours = (meeting.EndOfTheMeeting - meeting.StartOfTheMeeting).TotalHours;
                //var byteLength = (int)Math.Ceiling(totalHours);
                var availability = new byte[(int)totalHours];
                
                var userMeeting = new Models.Meetings.UserMeeting.UserMeeting
                {
                    Id = Guid.NewGuid(),
                    UserId = userMeetingDtoCreate.UserId,
                    MeetingId = userMeetingDtoCreate.MeetingId,
                    Status = InvitationStatus.Pending,
                    Availability = availability
                };
                await session.SaveAsync(userMeeting);
                await transaction.CommitAsync();
                return userMeeting;
            }
        }
    }

    public async Task<bool> DeleteUserMeetingEntity(Guid userMeetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var userMeeting = session.Get<Models.Meetings.UserMeeting.UserMeeting>(userMeetingId);
                    if (userMeeting == null) return false;

                    await session.DeleteAsync(userMeeting);
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
    
    public async Task<bool> DeleteUserMeetingEntity(Guid userId, Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var userMeeting = session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                        .Where(um => um.MeetingId == meetingId && um.UserId == userId).FirstOrDefaultAsync();
                    if (userMeeting == null) return false;
                    await session.DeleteAsync(userMeeting);
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
    
    public async Task<Guid?> GetUserMeetingId(Guid userId, Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var userMeeting = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (userMeeting == null) return null;
            return userMeeting.Id;
        }
    }

    public async Task<InvitationStatus?> GetUserMeetingStatus(Guid userId, Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var userMeeting = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (userMeeting == null) return null;
            return userMeeting.Status;
        }
    }
    
   public async Task<Models.Meetings.UserMeeting.UserMeeting> ChangeInvitationStatus(Guid userMeetingId, InvitationStatus status)
    {
        using(var session = NHibernateHelper.OpenSession())
        {
            using(var transaction = session.BeginTransaction())
            {
                try
                {
                    var userMeeting = session.Get<Models.Meetings.UserMeeting.UserMeeting>(userMeetingId);
                    if (userMeeting == null) return null;

                    userMeeting.Status = status;
                    await session.UpdateAsync(userMeeting);
                    await transaction.CommitAsync();
                    return userMeeting;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }
   
    public async Task<Models.Meetings.UserMeeting.UserMeeting> ChangeAvailbilityTimes(Guid userMeetingId, byte[] times)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var userMeeting = session.Get<Models.Meetings.UserMeeting.UserMeeting>(userMeetingId);
                    if (userMeeting == null) return null;

                    userMeeting.Availability = times;
                    await session.UpdateAsync(userMeeting);
                    await transaction.CommitAsync();
                    return userMeeting;

                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }

    public async Task<Models.Meetings.UserMeeting.UserMeeting> ChangeAvailbilityTimeFrames(Guid userMeetingId, int offset)
    {
        using (var session = _sessionFactory.OpenSession())
    {
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userMeeting = await session.GetAsync<Models.Meetings.UserMeeting.UserMeeting>(userMeetingId);
                if (userMeeting == null) return null;

                int bitLength = userMeeting.Availability.Length * 8;
                byte[] newAvailability = new byte[userMeeting.Availability.Length];

                if (offset < 0)
                {
                    // Przesunięcie w lewo
                    int shift = -offset;
                    for (int i = 0; i < bitLength; i++)
                    {
                        int fromIndex = i + shift;
                        if (fromIndex < bitLength)
                        {
                            int fromByteIndex = fromIndex / 8;
                            int fromBitIndex = fromIndex % 8;

                            int toByteIndex = i / 8;
                            int toBitIndex = i % 8;

                            if ((userMeeting.Availability[fromByteIndex] & (1 << fromBitIndex)) != 0)
                            {
                                newAvailability[toByteIndex] |= (byte)(1 << toBitIndex);
                            }
                        }
                    }
                }
                else if (offset > 0)
                {
                    // Przesunięcie w prawo
                    int shift = offset;
                    for (int i = bitLength - 1; i >= 0; i--)
                    {
                        int fromIndex = i - shift;
                        if (fromIndex >= 0)
                        {
                            int fromByteIndex = fromIndex / 8;
                            int fromBitIndex = fromIndex % 8;

                            int toByteIndex = i / 8;
                            int toBitIndex = i % 8;

                            if ((userMeeting.Availability[fromByteIndex] & (1 << fromBitIndex)) != 0)
                            {
                                newAvailability[toByteIndex] |= (byte)(1 << toBitIndex);
                            }
                        }
                    }
                }

                userMeeting.Availability = newAvailability;

                await session.UpdateAsync(userMeeting);
                await transaction.CommitAsync();
                return userMeeting;
            }
            catch
            {
                transaction.Rollback();
                return null;
            }
        }
    }
    }
    
    public async Task<bool> IsUserInMeeting(Guid userId, Guid meetingId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            return await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .AnyAsync(x => x.UserId == userId && x.MeetingId == meetingId);
        }
    }
    
    public async Task<string?> GetUserTimes(Guid userId, Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var userMeeting = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (userMeeting == null) return null;
            if (userMeeting.Availability is { } availabilityBytes)
            {
                StringBuilder binaryString = new StringBuilder();

                foreach (var b in availabilityBytes)
                {
                    // Konwersja każdego bajtu na binarną reprezentację
                    binaryString.Append(Convert.ToString(b, 2).PadLeft(1, '0'));
                }

                return binaryString.ToString();
            }
        
            return null;
        }
    }
    
    public async Task<List<AvailabilityTimesDTO>> GetAllUserTimes(Guid meetingId, Guid ownerId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var userMeeting = await session.Query<Models.Meetings.UserMeeting.UserMeeting>()
                .Where(x => x.MeetingId == meetingId)
                .ToListAsync();

            if (userMeeting == null) return null;
            
            var availabilityList = new List<AvailabilityTimesDTO>();
            foreach (var user in userMeeting)
            {
                if (user.Availability is { } availabilityBytes)
                {
                    StringBuilder binaryString = new StringBuilder();

                    foreach (var b in availabilityBytes)
                    {
                        // Konwersja każdego bajtu na binarną reprezentację
                        binaryString.Append(Convert.ToString(b, 2).PadLeft(1, '0'));
                    }

                    availabilityList.Add(new AvailabilityTimesDTO()
                    {
                        Availability = binaryString.ToString(),
                        UserId = user.UserId,
                        IsOwner = user.UserId.Equals(ownerId) ? true : false
                    });
                }
            }

            return availabilityList;
        }
    }
}