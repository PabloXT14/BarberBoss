using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return Ok();
    }
}