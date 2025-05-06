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

public class AuthService(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;


    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        //suggested by chat gpt - create a user that will be used in the signin overload instead of adding the email to the overload
        var user = await _userManager.FindByEmailAsync(formData.Email!);
        if (user == null)
        {
            return new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid Email or password." };
        }

        var result = await _signInManager.PasswordSignInAsync(user, formData.Password!, formData.IsPersistent, lockoutOnFailure: false);


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

       
    }


    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };
        var email = formData.Email.Trim().ToLower();

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "An account with this email already exists." };
       
        var userEntity = new UserEntity
        {
            UserName = formData.Email,
            FirstName = formData.FirstName,
            LastName = formData.LastName,
            Email = formData.Email,
        
        };
        
        var result = await _userManager.CreateAsync(userEntity, formData.Password);
        return result.Succeeded
           ? new AuthResult { Succeeded = true, StatusCode = 201 }
           : new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not user created"};
    }


    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }


}