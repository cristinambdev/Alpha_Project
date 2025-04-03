using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

//[Authorize]
public class AdminController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }


    [Route("members")]
    public IActionResult Users()
    {
        return View();
     
    }

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
