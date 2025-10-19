using Microsoft.AspNetCore.Mvc;

namespace AJT.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AllowRoleAttribute : TypeFilterAttribute
    {
        public string[] Roles { get; }
        public AllowRoleAttribute(params string[] roles)
        : base(typeof(AllowRoleFilter))
        {
            Roles = roles;
            Arguments = [roles];
        }
    }
}
