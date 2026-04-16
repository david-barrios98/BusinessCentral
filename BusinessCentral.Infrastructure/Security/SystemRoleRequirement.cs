using Microsoft.AspNetCore.Authorization;

namespace BusinessCentral.Infrastructure.Security
{
    public class SystemRoleRequirement : IAuthorizationRequirement
    {
        public string? RequiredRoleName { get; }

        public SystemRoleRequirement(string? requiredRoleName = null)
        {
            RequiredRoleName = requiredRoleName;
        }
    }
}