using Microsoft.AspNetCore.Mvc;

namespace AJT.Controllers
{
    [ApiController]
    [Route("ajt")]
    public class AJTController : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> GetTokens()
        {
            return Ok();
        }
    }
}
