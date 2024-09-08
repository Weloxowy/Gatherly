using gatherly.server.Entities.Meetings;

namespace gatherly.server.Models.Meetings.Invitations;

public interface IInvitationsRepository
{
    public Task<Invitations> CreateInvitation(InvitationDTO invitation);
    public Task<Invitations> GetInvitationById(Guid invitationId);
    public Task<List<InvitationDTOGetInvited>> GetAllInvitationsByMeetingId(Guid meetingId);
    public Task<List<InvitationDTOGetByUser>> GetAllInvitationsByUserId(Guid userId);
    public Task<bool> DeleteInvitation(Guid invitationId);
}