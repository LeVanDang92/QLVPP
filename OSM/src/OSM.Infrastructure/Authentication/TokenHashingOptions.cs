using System.ComponentModel.DataAnnotations;

namespace OSM.Infrastructure.Authentication
{
    public sealed class TokenHashingOptions
    {
        public const string SectionName = "TokenHashing";

        [Required]
        [MinLength(32)]
        public string Pepper { get; init; } = string.Empty;
    }
}
