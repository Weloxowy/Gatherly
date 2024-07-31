using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;

namespace gatherly.server.Persistence.Mailing;

public class MailEntityService : IMailEntityService
{
    private readonly IMailEntityRepository _mailEntityRepository;

    public MailEntityService(IMailEntityRepository mailEntityRepository)
    {
        _mailEntityRepository = mailEntityRepository;
    }

    public Task SendSsoCodeEmailAsync(UserEntity user, SsoSession ssoSession)
    {
       return _mailEntityRepository.SendSsoCodeEmailAsync(user, ssoSession);
    }

    public Task SendRecoveryEmailAsync(UserEntity user, RecoverySession session)
    {
        return _mailEntityRepository.SendRecoveryEmailAsync(user, session);
    }
    
}