using Microsoft.AspNetCore.Mvc;

namespace AJT.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class RequireAuthorizationAttribute : TypeFilterAttribute
    {
        public RequireAuthorizationAttribute(Type RequireAuthorizationFilter) : base(RequireAuthorizationFilter)
        {
        }
    }
}
