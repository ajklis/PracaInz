using AJT.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AJT.Controllers
{
    [ApiController]
    [Route("ajt")]
    public class AJTController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly JsonSerializerSettings _jsonSettings;

        public AJTController(ILoginService loginService)
        {
            _loginService = loginService;
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var combinedToken = await _loginService.Login(request.Login, request.Password);

            if (combinedToken is null)
                return Unauthorized();

            return Ok(JsonConvert.SerializeObject(combinedToken, _jsonSettings));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _loginService.Register(request.Username, request.Email, request.Password))
                return Ok();

            return BadRequest();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var combinedToken = await _loginService.Refresh(request.RefreshToken);

            if (combinedToken is null)
                return Unauthorized();

            return Ok(JsonConvert.SerializeObject(combinedToken, _jsonSettings));
        }


        public sealed class LoginRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }
        public sealed class RegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public sealed class RefreshRequest
        {
            public string RefreshToken { get; set; }
        }
    }
}
