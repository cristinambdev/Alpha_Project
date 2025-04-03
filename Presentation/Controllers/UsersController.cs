using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class UsersController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Route("members")]
    public async Task<IActionResult> Index()

    {
        var members = await _userService.GetUsersAsync();
        return View("Index",members);  // suggested by Chat GPT to explicitly specigy "Members" to ensure the correct view is used as there was mismatch  
    }

    [HttpPost]
    public IActionResult AddMember(AddMemberViewModel form)
    {
        if (!ModelState.IsValid)
        //manage information through API
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary( // kvp - key value pair , JSON object
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                 );
            return BadRequest(new { success = false, errors });
        }
        // Send Data to memberService
        return Ok(new { success = true });

        //with "MemberService"
        //var result = await _memberService.AddMemberAsync(form);
        //if (result)
        //{
        //    return Ok(new { success = true });
        //}
        //else
        //{
        //    return Problem("Unable to submit data");
        //}

    }


    [HttpPost]
    public IActionResult EditMember(EditMemberViewModel form)
    {
        if (!ModelState.IsValid)
        //manage information through API
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary( // kvp - key value pair , JSON object
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                 );
            return BadRequest(new { success = false, errors });
        }
        // Send Data to memberService

        return Ok(new { success = true });

        ////with "MemberService"
        //var result = await _membService.UpdateMemberAsync(form);
        //if (result)
        //{
        //    return Ok(new { success = true });
        //}
        //else
        //{
        //    return Problem("Unable to edit data");
        //}
    }

    //public async Task<IActionResult> GetMembers()
    //{
    //    var members = await _memberService.GetAllMembers();
    //    return View(members);
    //}
}
