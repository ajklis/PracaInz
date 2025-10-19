using AJT.Contracts;
using AJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace AJT.Attributes
{
    internal sealed class AllowRoleFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;
        private readonly IHashingService _hashingService;
        private readonly IRoleService _roleService;

        public AllowRoleFilter(string[] roles, IHashingService hashingService, IRoleService roleService)
        {
            _roles = roles;
            _hashingService = hashingService;
            _roleService = roleService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();
            if (!_hashingService.VerifiyHash(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var payloadJson = _hashingService.DecodePayload(token);
                var tokenObject = JsonConvert.DeserializeObject<Token>(payloadJson);
                if (tokenObject is null || string.IsNullOrEmpty(tokenObject.UserRoles))
                    throw new Exception();

                var userRoles = await _roleService.DecodeRoles(tokenObject.UserRoles);
                foreach (var role in _roles)
                    if (userRoles.Contains(role))
                        return;
            }
            catch (Exception e)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await Task.CompletedTask;
        }
    }
}
