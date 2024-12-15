using prometeyapi.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace prometeyapi.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(Role role) : base()
    {
        Roles = role.ToString();
    }
}