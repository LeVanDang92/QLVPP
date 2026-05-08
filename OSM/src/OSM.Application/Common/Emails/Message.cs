using MimeKit;

namespace OSM.Application.Common.Emails
{
    public sealed class Message
    {
        public Message(
            IEnumerable<string> to,
            string subject,
            string content,
            IEnumerable<string>? cc = null)
        {
            To = to.Select(x => new MailboxAddress("mail", x)).ToList();
            Cc = cc?.Select(x => new MailboxAddress("mail", x)).ToList() ?? [];
            Subject = subject;
            Content = content;
        }

        public Message(
            IEnumerable<string> to,
            IEnumerable<string> cc,
            string subject,
            string content)
            : this(to, subject, content, cc)
        {
        }

        public List<MailboxAddress> To { get; }

        public List<MailboxAddress> Cc { get; }

        public string Subject { get; }

        public string Content { get; }
    }
}
