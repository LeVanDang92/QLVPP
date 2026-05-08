using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace OSM.Infrastructure.Authorization;

public sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var existingPolicy = await base.GetPolicyAsync(policyName);

        if (existingPolicy is not null)
        {
            return existingPolicy;
        }

        if (!IsPermissionPolicy(policyName))
        {
            return null;
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }

    private static bool IsPermissionPolicy(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            return false;
        }

        // Ví dụ hợp lệ:
        // products.read
        // products.write
        // products.delete
        // suppliers.read
        var parts = policyName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return parts.Length == 2;
    }
}