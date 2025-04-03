using Domain.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Models;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormData formData);
    Task<AuthResult> SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;


    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        // Add debug logging
        Console.WriteLine($"Attempting login for: {formData.Email}");

        var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);

        // Additional logging
        Console.WriteLine($"Login result: Succeeded={result.Succeeded}, IsLockedOut={result.IsLockedOut}, IsNotAllowed={result.IsNotAllowed}");


        // Additional logging
        Console.WriteLine($"Login result: Succeeded={result.Succeeded}, IsLockedOut={result.IsLockedOut}, IsNotAllowed={result.IsNotAllowed}");

        if (!result.Succeeded)
        {
            string errorMessage = "Invalid Email or password.";
            if (result.IsLockedOut)
                errorMessage = "Account is locked out.";
            else if (result.IsNotAllowed)
                errorMessage = "Account is not allowed to sign in.";
            else if (result.RequiresTwoFactor)
                errorMessage = "Two-factor authentication required.";
            return new AuthResult { Succeeded = false, StatusCode = 401, Error = errorMessage };
        }

        return new AuthResult { Succeeded = true, StatusCode = 200 };

        //return result.Succeeded
        //  ? new AuthResult { Succeeded = true, StatusCode = 200 }
        //  : new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid Email or password." };
    }


    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _userService.CreateUserAsync(formData);
        return result.Succeeded
           ? new AuthResult { Succeeded = true, StatusCode = 201 }
           : new AuthResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }


    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }
}