namespace gatherly.server.Entities.Mailling.EmailMessage;

public class EmailMessage
{
    public virtual string To { get; set; }
    public virtual string Subject { get; set; }
    public virtual string Body { get; set; }
    public virtual bool IsHtml { get; set; }
}