using System.ComponentModel.DataAnnotations;

namespace OSM.Application.Common.Emails
{
    public sealed class EmailConfiguration
    {
        public const string SectionName = "EmailConfiguration";

        [EmailAddress]
        public string From { get; init; } = string.Empty;

        public string SmtpServer { get; init; } = string.Empty;

        [Range(1, 65535)]
        public int Port { get; init; } = 25;

        public string UserName { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
