using AJT.Contracts;
using AJT.Exceptions;
using AJT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace AJT.Attributes
{
    internal sealed class RequireAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly IHashingService _hashingService;

        public RequireAuthenticationFilter(IHashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Unauthorized" }));
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();

            if (!_hashingService.VerifiyHash(token))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Unauthorized" }));
                return;
            }

            try
            {
                var payloadJson = _hashingService.DecodePayload(token);
                var tokenObject = JsonConvert.DeserializeObject<Token>(payloadJson);
                if (tokenObject is null || string.IsNullOrEmpty(tokenObject.UserRoles))
                    throw new UnauthorizedException();
                var data = tokenObject.Data;
                if (data is not null)
                    context.HttpContext.Items["AJT"] = data;

                if (tokenObject.ExpirationDate < DateTime.Now)
                {
                    throw new UnauthorizedException();
                }
            }
            catch
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Unauthorized" }));
                return;
            }

            await Task.CompletedTask;

        }
    }
}
