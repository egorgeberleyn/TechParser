using Microsoft.AspNetCore.Mvc;
using TechParser.Storage;

namespace TechParser.Controllers;

public class MetallPortalController : Controller
{
    [HttpGet ("Test")]
    public async Task<ActionResult> Test()
    {
        return Ok();
    }
}