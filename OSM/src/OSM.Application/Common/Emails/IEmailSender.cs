namespace OSM.Application.Common.Emails
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmail(Message message);
        void SendEmailHtml(Message message);

        void SendEmailWithAttack(Message message, string attachmentPath);
        bool SendEmailHtmlWithAttack(Message message, string attachmentPath);
    }
}
