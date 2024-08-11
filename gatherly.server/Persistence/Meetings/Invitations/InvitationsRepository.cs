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

    public async Task<Models.Meetings.Invitations.Invitations> CreateInvitation(InvitationDTOCreate invitation)
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
                    ValidTime = DateTime.Now.AddDays(7)
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

    public Task<List<Models.Meetings.Invitations.Invitations>> GetAllInvitationsByMeetingId(Guid meetingId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            return session.Query<Models.Meetings.Invitations.Invitations>()
                .Where(x => x.MeetingId == meetingId).ToListAsync();
        }
    }

    public Task<List<Models.Meetings.Invitations.Invitations>> GetAllInvitationsByUserId(Guid userId)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            return session.Query<Models.Meetings.Invitations.Invitations>()
                .Where(x => x.UserId == userId).ToListAsync();
        }
    }
    
}