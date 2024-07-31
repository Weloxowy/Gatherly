using DotNetEnv;
using FluentEmail.Core;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using NHibernate;

namespace gatherly.server.Persistence.Mailing.EmailTemplates;

public class MailEntityRepository : IMailEntityRepository
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IFluentEmail _fluentEmail;

    public MailEntityRepository(ISessionFactory sessionFactory, IFluentEmail fluentEmail)
    {
        _sessionFactory = sessionFactory;
        _fluentEmail = fluentEmail;
    }
    
    public async Task SendRecoveryEmailAsync(UserEntity user, RecoverySession session)
    {
        var smtpAddress = Env.GetString("SMTP_ADDRESS");
        var portNumber = Env.GetInt("PORT_NUMBER");
        var mailLogin = Env.GetString("MAIL_LOGIN");
        var mailPassword = Env.GetString("MAIL_PASSWORD");

        if (string.IsNullOrEmpty(smtpAddress) || portNumber == 0 || string.IsNullOrEmpty(mailLogin) || string.IsNullOrEmpty(mailPassword))
        {
            throw new Exception("SMTP configuration is invalid.");
        }
        try
        {
            var response = await _fluentEmail.To(user.Email)
                .Subject("Recovery of your account")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Persistence/Mailing/EmailTemplates/RecoverAccount.cshtml", new
                {
                    Name = user.Name,
                    Code = session.Id
                }).SendAsync();

            if (!response.Successful)
            {
                throw new Exception(string.Join(", ", response.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            throw new Exception("There was an error sending the email.", ex);
        }
    }

    public async Task SendSsoCodeEmailAsync(UserEntity user, SsoSession ssoSession)
    {
        var smtpAddress = Env.GetString("SMTP_ADDRESS");
        var portNumber = Env.GetInt("PORT_NUMBER");
        var mailLogin = Env.GetString("MAIL_LOGIN");
        var mailPassword = Env.GetString("MAIL_PASSWORD");

        if (string.IsNullOrEmpty(smtpAddress) || portNumber == 0 || string.IsNullOrEmpty(mailLogin) || string.IsNullOrEmpty(mailPassword))
        {
            throw new Exception("SMTP configuration is invalid.");
        }

        try
        {
            var response = await _fluentEmail.To(ssoSession.UserEmail)
                .Subject("Your SSO Verification Code")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Persistence/Mailing/EmailTemplates/SsoCode.cshtml", new
                {
                    Name = user.Name,
                    VerificationCode = ssoSession.VerificationCode
                }).SendAsync();

            if (!response.Successful)
            {
                throw new Exception(string.Join(", ", response.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            throw new Exception("There was an error sending the email.", ex);
        }
    }
}