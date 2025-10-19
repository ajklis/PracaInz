using AJT.Attributes;
using AJT.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IRoleService _roleService;

        public ApiController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("role/grant")]
        public async Task<IActionResult> GrantRole([FromBody] GrantRequest request)
        {
            await _roleService.AddUserRole(request.UserId, request.Role);
            return Ok();
        }

        [HttpPost("role/remove")]
        public async Task<IActionResult> RemoveRole([FromBody] GrantRequest request)
        {
            await _roleService.RemoveUserRole(request.UserId, request.Role);
            return Ok();
        }

        [AllowRole("admin")]
        [HttpGet("admin")]
        public IActionResult Admin() => Ok("admin");

        [AllowRole("admin", "role1")]
        [HttpGet("admin1")]
        public IActionResult Admin1() => Ok("admin and role1");

        [AllowRole("role1")]
        [HttpGet("role1")]
        public IActionResult Role1() => Ok("role1");

        [AllowRole("role2")]
        [HttpGet("role2")]
        public IActionResult Role2() => Ok("role2");


        public sealed class GrantRequest
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }
        }
            
    }
}
