using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Models.Mailing.MailEntity;

public interface IMailEntityService
{
    Task SendSsoCodeEmailAsync(UserEntity user, SsoSession ssoSession);
    Task SendRecoveryEmailAsync(UserEntity user, RecoverySession session);


}