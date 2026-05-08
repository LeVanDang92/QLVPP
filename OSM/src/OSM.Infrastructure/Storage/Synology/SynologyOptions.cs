using System.ComponentModel.DataAnnotations;

namespace OSM.Infrastructure.Storage.Synology
{
    public sealed class SynologyOptions
    {
        public const string SectionName = "Synology";

        [Required]
        [Url]
        public string BaseUrl { get; init; } = string.Empty;

        [Required]
        public string Account { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;

        public string Session { get; init; } = "FileStation";

        public int AuthVersion { get; init; } = 3;

        public int FileStationVersion { get; init; } = 2;
    }
}
