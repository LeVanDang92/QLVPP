using Microsoft.AspNetCore.Authorization;

namespace OSM.Infrastructure.Authorization
{
    public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
