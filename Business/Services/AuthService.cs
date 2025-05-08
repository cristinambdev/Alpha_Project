using Domain.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Models;
using System.Security.Claims;
using Microsoft.SqlServer.Server;


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
        if(result.Succeeded)
        { // with chat gpt help
            var roles = await _userManager.GetRolesAsync(user);

            string displayName = $"{user.FirstName} {user.LastName}";
            string displayRole = string.Join(", ", roles);
            string displayImage = user.UserImage ?? "";

            // Call AddClaimByEmailAsync to add the claims
            await AddClaimByEmailAsync(user.Email!, "DisplayName", displayName, "", "");
            await AddClaimByEmailAsync(user.Email!, "DisplayRole", displayRole, "", "");
            if (!string.IsNullOrEmpty(displayImage))
            {
                await AddClaimByEmailAsync(user.Email!, "image", displayImage, "", "");
            }

        }


        return new AuthResult { Succeeded = true, StatusCode = 200 };

    }

    public async Task AddClaimByEmailAsync(string email, string typeName, string value, string typeRole, string typeImage)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            // Add DisplayName claim 
            if (!claims.Any(x => x.Type == typeName))
            {
                await _userManager.AddClaimsAsync(user, new List<Claim> { new Claim(typeName, value) });// by chat gpt  wrap each Claim object in a collection before passing it to AddClaimsAsync
            }
            // Add Image claim 
            if (typeName.ToLower() == "image" && !claims.Any(x => x.Type == typeName && x.Value == value)) // Check typeName for "image"

                {
                    await _userManager.AddClaimsAsync(user, new List<Claim> { new Claim(typeImage, value) });
            }

            // Add Role claim 
            if (typeName.ToLower() == "displayrole" && !claims.Any(x => x.Type == "DisplayRole")) // Hardcoded "DisplayRole" type
            {
                var roles = await _userManager.GetRolesAsync(user); //by chat gpt to get role claims
                if (roles.Any())
                {
                    var displayRole = string.Join(", ", roles);

                    await _userManager.AddClaimsAsync(user, new List<Claim> { new Claim("DisplayRole", displayRole) });
                }
            }
        }
     
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