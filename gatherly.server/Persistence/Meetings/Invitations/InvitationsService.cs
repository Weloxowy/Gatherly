using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.Invitations;

namespace gatherly.server.Persistence.Meetings.Invitations;

public class InvitationsService : IInvitationsService
{
    private readonly InvitationsRepository _invitationsRepository = new(NHibernateHelper.SessionFactory);
    
    public Task<Models.Meetings.Invitations.Invitations> CreateInvitation(InvitationDTO invitation)
    {
      return _invitationsRepository.CreateInvitation(invitation);
    }

    public Task<Models.Meetings.Invitations.Invitations> GetInvitationById(Guid invitationId)
    {
        return _invitationsRepository.GetInvitationById(invitationId);
    }

    public Task<List<InvitationDTOGetInvited>> GetAllInvitationsByMeetingId(Guid meetingId)
    {
        return _invitationsRepository.GetAllInvitationsByMeetingId(meetingId);
    }

    public Task<List<InvitationDTOGetByUser>> GetAllInvitationsByUserId(Guid userId)
    {
        return _invitationsRepository.GetAllInvitationsByUserId(userId);
    }

    public Task<bool> DeleteInvitation(Guid invitationId)
    {
        return _invitationsRepository.DeleteInvitation(invitationId);
    }
}