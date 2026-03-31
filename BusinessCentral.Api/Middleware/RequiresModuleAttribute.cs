namespace BusinessCentral.Api.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresModuleAttribute : Attribute
    {
        public string ModuleName { get; }
        public RequiresModuleAttribute(string moduleName) => ModuleName = moduleName;
    }
}
