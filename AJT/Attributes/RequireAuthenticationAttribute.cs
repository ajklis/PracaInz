using Microsoft.AspNetCore.Mvc;

namespace AJT.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAuthenticationAttribute : TypeFilterAttribute
    {
        public RequireAuthenticationAttribute() : base(typeof(RequireAuthenticationFilter))
        {
        }
    }
}
