namespace gatherly.server.Models.Mailing.MailEntity;

public class MailEntity
{
    public MailEntity()
    {
    }

    public MailEntity(string to, string subject, string body, bool isHtml)
    {
        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }

    public virtual string To { get; set; }
    public virtual string Subject { get; set; }
    public virtual string Body { get; set; }
    public virtual bool IsHtml { get; set; }
}