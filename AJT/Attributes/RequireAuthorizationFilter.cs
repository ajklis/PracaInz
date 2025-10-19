using AJT.Contracts;
using AJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace AJT.Attributes
{
    internal sealed class RequireAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IHashingService _hashingService;

        public RequireAuthorizationFilter(IHashingService hashingService)
        {
            _hashingService = hashingService;
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
                var data = GetDataFromTokenJson(payloadJson);
                if (data is not null)
                    context.HttpContext.Items["AJT"] = data; 
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await Task.CompletedTask;

        }

        private object? GetDataFromTokenJson(string json)
        {
            try
            {
                var token = JsonConvert.DeserializeObject<Token>(json);
                var data = token?.Data;
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
