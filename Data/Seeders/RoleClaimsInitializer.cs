//using Microsoft.AspNetCore.Identity;
//using System.Security.Claims;
//using Data.Constants;

//namespace Data.Seeders;

//public class RoleClaimsInitializer(RoleManager<IdentityRole> roleManager)
//{
//    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

//    public async Task SeedRoleClaims()
//    {
//        var adminRole = await _roleManager.FindByNameAsync("Admin");
//        var userRole = await _roleManager.FindByNameAsync("User");

//        // Admin Permissions
//        if (adminRole != null)
//        {
//            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.Permission, ClaimConstants.CreateUser));
//            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.Permission, ClaimConstants.EditUser));
//            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.Permission, ClaimConstants.DeleteUser));
//            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.Permission, ClaimConstants.ViewUser));
//            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.Permission, ClaimConstants.ViewAllUsers));
//            // ... other admin permissions ...
//        }

//        // User Permissions
//        if (userRole != null)
//        {
//            await _roleManager.AddClaimAsync(userRole, new Claim(ClaimConstants.Permission, ClaimConstants.ViewOwnProfile));
//            await _roleManager.AddClaimAsync(userRole, new Claim(ClaimConstants.Permission, ClaimConstants.EditOwnProfile));
//            await _roleManager.AddClaimAsync(userRole, new Claim(ClaimConstants.Permission, ClaimConstants.ViewAssignedProjects));
//            await _roleManager.AddClaimAsync(userRole, new Claim(ClaimConstants.Permission, ClaimConstants.ViewProjectDetails));
//            // ... other user permissions ...
//        }
//    }
//}
