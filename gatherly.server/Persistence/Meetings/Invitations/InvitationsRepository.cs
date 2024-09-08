using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.Invitations;
using NHibernate;
using NHibernate.Linq;

namespace gatherly.server.Persistence.Meetings.Invitations;

public class InvitationsRepository : IInvitationsRepository
{
    private readonly ISessionFactory _sessionFactory;

    public InvitationsRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task<Models.Meetings.Invitations.Invitations> CreateInvitation(InvitationDTO invitation)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                if (invitation.MeetingId == invitation.UserId) return null;

                var existingInvitation = await session.Query<Models.Meetings.Invitations.Invitations>()
                    .SingleOrDefaultAsync(x => x.MeetingId == invitation.MeetingId && x.UserId == invitation.UserId);

                if (existingInvitation != null) return null;
                var newInvitation = new Models.Meetings.Invitations.Invitations
                {
                    MeetingId = invitation.MeetingId,
                    UserId = invitation.UserId,
                    ValidTime = DateTime.UtcNow.AddDays(7)
                };
                
                await session.SaveAsync(newInvitation);
                await transaction.CommitAsync();
                return newInvitation;
            }
        }
    }

    public async Task<Models.Meetings.Invitations.Invitations> GetInvitationById(Guid invitationId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var invitation = await session.GetAsync<Models.Meetings.Invitations.Invitations>(invitationId);
            if (invitation == null)
            {
                return null;
            }
            return invitation;
        }
    }
    
    public async Task<bool> DeleteInvitation(Guid invitationId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var existingInvitation = await session.Query<Models.Meetings.Invitations.Invitations>()
                    .SingleOrDefaultAsync(x => x.Id == invitationId);

                if (existingInvitation == null) return false;
                

                await session.DeleteAsync(existingInvitation);
                await transaction.CommitAsync();
                return true;
            }
        }
    }

    public async Task<List<InvitationDTOGetInvited>> GetAllInvitationsByMeetingId(Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            // Query with join to get invitations and corresponding users
            var invitations = await session.Query<Models.Meetings.Invitations.Invitations>()
                .Where(i => i.MeetingId == meetingId).Join(session.Query<Models.Authentication.UserEntity.UserEntity>(), 
                    invitation => invitation.UserId, 
                    user => user.Id, 
                    (invitation, user) => new Entities.Meetings.InvitationDTOGetInvited
                    {
                        Id = invitation.Id,
                        UserId = invitation.UserId,
                        UserName = user.Name, // Mapping username from the user entity
                        UserAvatar = user.AvatarName,
                        MeetingId = invitation.MeetingId,
                        ValidTime = invitation.ValidTime
                    })
                .ToListAsync();

            return invitations;
        }
    }

    public async Task<List<InvitationDTOGetByUser>> GetAllInvitationsByUserId(Guid userId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            // Query with join to get invitations and corresponding meeting details
            var invitations = await session.Query<Models.Meetings.Invitations.Invitations>()
                .Where(i => i.UserId == userId)
                .Join(
                    session.Query<Models.Meetings.Meeting.Meeting>(), // Assuming you have a Meeting entity
                    invitation => invitation.MeetingId,
                    meeting => meeting.Id,
                    (invitation, meeting) => new Entities.Meetings.InvitationDTOGetByUser
                    {
                        InvitationId = invitation.Id,
                        UserId = invitation.UserId,
                        MeetingId = invitation.MeetingId,
                        ValidTime = invitation.ValidTime,
                        OwnerId = meeting.OwnerId,
                        MeetingName = meeting.MeetingName,
                        Description = meeting.Description,
                        PlaceName = meeting.PlaceName,
                        Lon = meeting.Lon,
                        Lat = meeting.Lat,
                        StartOfTheMeeting = meeting.StartOfTheMeeting,
                        EndOfTheMeeting = meeting.EndOfTheMeeting,
                        CreationTime = meeting.CreationTime,
                        IsMeetingTimePlanned = meeting.IsMeetingTimePlanned,
                        TimeZone = meeting.TimeZone
                    }
                )
                .ToListAsync();

            return invitations;
        }
    }


    
}