using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)  : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;



    #region SignUp
    [HttpGet]
    public ActionResult SignUp(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var user = new UserEntity { UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };
        var identityResult = await _userManager.CreateAsync(user, model.Password);
        if (identityResult.Succeeded) 
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
        }
        ModelState.AddModelError("Unable", "Unable to create user.");
        return View(model);

        }

    #endregion


    #region SignIn
    public IActionResult SignIn(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        if (ModelState.IsValid)
        {
            var signInFormData = model.MapTo<SignInFormData>();
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
 
                return LocalRedirect(returnUrl);
            }
        }
        ViewBag.ErrorMessage = "Incorrect email or password.";
        return View(model);
    }


    #endregion

    #region External Authentication

    [HttpGet]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if(string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid Provider");
            return View("SignIn");
        }
        var redirectUrl = Url.Action("ExternalSignInCallBack", "Auth", new {returnUrl})!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);

    }
    [HttpGet("signin‑google")]
    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");

        if(!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("SignIn");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("SignIn");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if(signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string firstName = string.Empty;
            string lastName = string.Empty;
            try
            {
                firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                lastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!;
            }
            catch { }

            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            string username = $"ext_{info.LoginProvider.ToLower()}_{email}";


            var user = new UserEntity { UserName = username, Email = email, FirstName = firstName, LastName = lastName };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("SignIn");
        }
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }
}
