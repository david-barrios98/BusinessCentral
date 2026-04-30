namespace BusinessCentral.Api.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequiresPermissionAttribute : Attribute
{
    public string Permission { get; }

    public RequiresPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}

