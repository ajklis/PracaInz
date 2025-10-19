using AJT.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AJT
{
    internal sealed class AuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IHashingService _hashingService;
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
                var payloadJson = JwtHelper.DecodePayload(token);
                context.HttpContext.Items["JwtPayload"] = payloadJson; // można potem odczytać w kontrolerze
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await Task.CompletedTask;

        }
    }
}
