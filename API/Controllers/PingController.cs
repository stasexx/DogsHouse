using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PingController : BaseController
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }
}