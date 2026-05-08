using Microsoft.AspNetCore.Identity;

namespace OSM.Infrastructure.Identity
{
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
