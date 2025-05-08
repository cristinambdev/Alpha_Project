using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IAuthService authService) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly IAuthService _authService = authService;



    #region Local SignUp
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
        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLower();
        //Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "An account with this email already exists.");
            return View(model);
        }
        // Prepare the form data for sign-up
        var formData = new SignUpFormData
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Password = model.Password
        };
        // Attempt to sign up the user
        var result = await _authService.SignUpAsync(formData);
        if(result.Succeeded)
        {
            //Get the newly created user and sign them in automatically
           var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // By chat gpt- Set the EmailConfirmed field to true, to be able to log in
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);  // Update the user in the database


                // Automatically sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            // If we can't sign in automatically for some reason, redirect to sign in page
            return RedirectToAction("SignIn", "Auth");
        }
        ViewBag.ErrorMessage = "Unable to create account. Please try again.";
        return View(model);

    }

    #endregion


    #region SignIn
    [HttpGet]
    public IActionResult SignIn(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
    {

        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "Incorrect email or password";

            return View(model);
        }

        var signInFormData = new SignInFormData
        {
            Email = model.Email,
            Password = model.Password,
            IsPersistent = model.IsPersistent
        };

       
        var result = await _authService.SignInAsync(signInFormData);

        if (result.Succeeded)
        {
            if(Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Overview");
        }

        // Show error message
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = result.Error ?? "Incorrect email or password.";
        return View(model);

    }
    #endregion

    #region External Authentication

    [HttpGet]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid Provider");
            return View("SignIn");
        }
        var redirectUrl = Url.Action("ExternalSignInCallBack", "Auth", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);

    }
    [HttpGet("signin‑google")]
    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("SignIn");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("SignIn");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
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
