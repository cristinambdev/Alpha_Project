using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers;

public class AuthController(IAuthService authService, UserManager<UserEntity> userManager) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly UserManager<UserEntity> _userManager = userManager;


    public ActionResult SignUp()
    {
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        ViewBag.ErrorMessage = null;



        var signUpFormData = model.MapTo<SignUpFormData>();

        var result = await _authService.SignUpAsync(signUpFormData);
        if (result.Succeeded)
        {
            return RedirectToAction("SignIn", "Auth");
        }
        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }


    public IActionResult SignIn(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
    {
        //ViewBag.ErrorMessage = "";
        //if (ModelState.IsValid) 
        //{
        //    var signInFormData = model.MapTo<SignInFormData>();
        //    var result = await _authService.SignInAsync(signInFormData);
        //    if(result.Succeeded) 
        //        return Redirect(returnUrl);
        //}

        //ViewBag.ErrorMessage = "Incorrect email or password.";
        //return View(model);

        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;

        // sugggested by Chat GPT Validate the return URL is local as a secutiry best practice.
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "~/";

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Incorrect email or password";
            return View(model);
        }



        var signInFormData = model.MapTo<SignInFormData>();

        // Find the user by email suggested by Chat GPT as my Log in wouldn't work
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && !user.EmailConfirmed)
        {
            ModelState.AddModelError("", "Please confirm your email before logging in.");
            return View(model);
        }

        var result = await _authService.SignInAsync(signInFormData);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }
}
