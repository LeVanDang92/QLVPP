using System.ComponentModel.DataAnnotations;

namespace OSM.Infrastructure.Persistence.Seed
{
    public sealed class IdentitySeedOptions
    {
        public const string SectionName = "IdentitySeed";

        public bool Enabled { get; init; }

        [Required]
        public string AdminUserName { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        public string AdminEmail { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string AdminPassword { get; init; } = string.Empty;
    }
}
