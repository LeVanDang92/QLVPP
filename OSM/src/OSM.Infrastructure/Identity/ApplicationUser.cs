using Microsoft.AspNetCore.Identity;

namespace OSM.Infrastructure.Identity
{
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }

        public string? PasswordShow { get; set; }

        public string Department { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
