using Microsoft.AspNetCore.Http;
using OSM.Application.Abstractions.Authentication;
using System.Security.Claims;

namespace OSM.Infrastructure.Authentication
{
    public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => httpContextAccessor.HttpContext?.User.Identity?.Name;
        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
