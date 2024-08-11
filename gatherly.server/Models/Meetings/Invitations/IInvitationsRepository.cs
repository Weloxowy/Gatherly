using gatherly.server.Entities.Meetings;

namespace gatherly.server.Models.Meetings.Invitations;

public interface IInvitationsRepository
{
    public Task<Invitations> CreateInvitation(InvitationDTOCreate invitation);
    public Task<Invitations> GetInvitationById(Guid invitationId);
    public Task<List<Invitations>> GetAllInvitationsByMeetingId(Guid meetingId);
    public Task<List<Invitations>> GetAllInvitationsByUserId(Guid userId);
    public Task<bool> DeleteInvitation(Guid invitationId);
}