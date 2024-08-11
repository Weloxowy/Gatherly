using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Meetings.Meeting;

namespace gatherly.server.Models.Mailing.MailEntity;

public interface IMailEntityRepository
{
    Task SendSsoCodeEmailAsync(UserEntity user, SsoSession ssoSession);

    Task SendRecoveryEmailAsync(UserEntity user, RecoverySession session);

    Task SendMeetingDeletedAsync(string userName, string userMail, string meetingName);

}