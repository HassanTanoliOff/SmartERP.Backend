using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartERP.API.Controllers;

[AllowAnonymous]
public class HealthCheckController : BaseController
{
    // GET
    public ActionResult GetHealth()
    {
        return Ok("Server Running");
    }
}
