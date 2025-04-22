using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class AdminController : Controller
{
    
    public IActionResult Index()
    {
        return RedirectToAction("Users");
    }

    //[Authorize(Roles = "admin")] //rol hantering
    [Route("members")]
    public IActionResult Users()
    {
        return View();
     
    }

    //[Authorize(Roles = "admin")] //rol hantering
    [Route("clients")]
    public IActionResult Clients()
    {
        return View();
    }
    
    [Route("projects")]
    public IActionResult Projects()
    {
        return View();
    }


}
