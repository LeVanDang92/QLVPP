using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OSM.Application.Common.Emails;

namespace OSM.Infrastructure.Emails
{
    public sealed class EmailSender(
        IOptions<EmailConfiguration> emailConfig,
        ILogger<EmailSender> logger) : IEmailSender
    {
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public void SendEmailWithAttack(Message message, string attachmentPath)
        {
            var emailMessage = CreateEmailMessage(message, attachmentPath);
            Send(emailMessage);
        }

        public void SendEmailHtml(Message message)
        {
            var emailMessage = CreateEmailMessageHtml(message);
            Send(emailMessage);
        }

        public bool SendEmailHtmlWithAttack(Message message, string attachmentPath)
        {
            var emailMessage = CreateEmailMessageHtml(message, attachmentPath);
            return Send(emailMessage);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SendEmail(new Message([email], subject, message));
            return Task.CompletedTask;
        }

        private MimeMessage CreateEmailMessage(Message message, string attachmentPath = "")
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("OSM", emailConfig.Value.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            if (string.IsNullOrWhiteSpace(attachmentPath))
            {
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            }
            else
            {
                var builder = new BodyBuilder { TextBody = message.Content };
                builder.Attachments.Add(attachmentPath);
                emailMessage.Body = builder.ToMessageBody();
            }

            return emailMessage;
        }

        private MimeMessage CreateEmailMessageHtml(Message message, string attachmentPath = "")
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("OSM", emailConfig.Value.From));

            if (message.Cc is not null && message.Cc.Any())
            {
                emailMessage.Cc.AddRange(message.Cc);
            }

            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            if (string.IsNullOrWhiteSpace(attachmentPath))
            {
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            }
            else
            {
                var builder = new BodyBuilder { HtmlBody = message.Content };
                builder.Attachments.Add(attachmentPath);
                emailMessage.Body = builder.ToMessageBody();
            }

            return emailMessage;
        }

        private bool Send(MimeMessage mailMessage)
        {
            if (string.IsNullOrWhiteSpace(emailConfig.Value.From) || string.IsNullOrWhiteSpace(emailConfig.Value.SmtpServer))
            {
                logger.LogWarning("Email is not configured. Subject: {Subject}", mailMessage.Subject);
                return false;
            }

            using var client = new SmtpClient();

            try
            {
                client.Connect(emailConfig.Value.SmtpServer, emailConfig.Value.Port, SecureSocketOptions.Auto);

                if (!string.IsNullOrWhiteSpace(emailConfig.Value.UserName))
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfig.Value.UserName, emailConfig.Value.Password);
                }

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email. Subject: {Subject}", mailMessage.Subject);
                return false;
            }
            finally
            {
                if (client.IsConnected)
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
