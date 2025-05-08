using Business.Services;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Presentation.Controllers;

//[Authorize(Roles = "Admin")]
public class UsersController(IUserService userService, AppDbContext context, IWebHostEnvironment env, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly AppDbContext _context = context;
    private readonly IWebHostEnvironment _env = env;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;


    [HttpGet]
    [Route("members")]
    public async Task<IActionResult> Index()
    {
        var userResult = await _userService.GetUsersAsync();

        var viewModel = new UsersViewModel()
        {
            Users = new List<UserViewModel>() 
        };

        foreach (var user in userResult?.Result!)
        {
            // Create a new instance of UserEntity
            var userEntity = new Data.Entities.UserEntity
            {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            UserImage = user.UserImage
            };

            // Get roles for the user
            var roles = await _userManager.GetRolesAsync(userEntity);
            var roleName = roles.FirstOrDefault(); // Handle multiple roles if needed

            // Add user data to the ViewModel
            viewModel.Users.Add(new UserViewModel
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                JobTitle = userEntity.JobTitle,
                UserImage = userEntity.UserImage,
                PhoneNumber = userEntity.PhoneNumber,
                Role = roleName!
            });
        }

        return View(viewModel);
    }

    [HttpGet]//By chat GPT so that the roles are fed into the form 
    public async Task<IActionResult> AddMember()
    {
        ViewBag.Roles = await _roleManager.Roles
            .Select(x => new SelectListItem
            {
                Value = x.Name,
                Text = x.Name
            }).ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(AddMemberViewModel form)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                 kvp => kvp.Key,
                 kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
               );

                ViewBag.Roles = await _roleManager.Roles
                    .Select(x => new SelectListItem
                    {
                        Value = x.Name,
                        Text = x.Name
                    }).ToListAsync();

                return BadRequest(new { success = false, errors });
            }

            // By Chat GPT. Check if the email already exists and retrieves message
            var existingUser = await _userManager.FindByEmailAsync(form.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with the same email already exists.");
                ViewBag.Roles = await _roleManager.Roles
                .Select(x => new SelectListItem { Value = x.Name, Text = x.Name })
                .ToListAsync();
                return BadRequest(new { success = false });
            }

            //  Handle the file upload
            string? imagePath = null; // suggested by chat GPT to store the image 
            if (form.UserImage != null)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "members");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.UserImage.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.UserImage.CopyToAsync(stream);
                }

                imagePath = $"/uploads/members/{fileName}";
            }

            var user = new UserEntity
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                UserName = form.Email,
                PhoneNumber = form.Phone,
                JobTitle = form.JobTitle,
                UserImage = imagePath,
                Address = new UserAddressEntity
                {
                    StreetName = form.StreetName!,
                    PostalCode = form.PostalCode!,
                    City = form.City!
                },
            };

            var result = await _userManager.CreateAsync(user, "TempPassword123!");

            if (!result.Succeeded)
            {

                return BadRequest(new { success = false, errors = result.Errors.Select(e => e.Description) });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, form.Role);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new { success = false, errors = roleResult.Errors.Select(e => e.Description) });
            }

            return Ok(new { success = true });

        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetUserData(string id) //with the help of Claude.Ai to get userdata that will be populated in the edit form
    {

        var user = await _userManager.Users
                                  .Include(u => u.Address) // Include the Address entity
                                  .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound();

        return Json(new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            phoneNumber = user.PhoneNumber,
            jobTitle = user.JobTitle,
            userImage = user.UserImage,
            role = user.Role,
            streetName = user.Address?.StreetName,
            postalCode = user.Address?.PostalCode,
            city = user.Address?.City,

        });

    }

    [HttpPost]
    public async Task<IActionResult> EditMember([FromForm] EditMemberViewModel form) // [FromForm] suggested by chat GPT as form is handling different formats
    {

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                 );
            return BadRequest(new { success = false, errors });
        }

        //// Handle image upload
        string? imagePath = null;// byt chat GPT - Keep existing image by default
        if (form.UserImage != null && form.UserImage.Length > 0)
        {
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "members");
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.UserImage.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.UserImage.CopyToAsync(stream);
            }

            imagePath = $"/uploads/members/{fileName}";
        }
        // Get the existing user with address
        var user = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == form.Id);

        if (user == null)
        {
            return NotFound();
        }
        // Update user properties
        user.FirstName = form.FirstName;
        user.LastName = form.LastName;
        user.Email = form.Email;
        user.UserName = form.Email;
        user.PhoneNumber = form.Phone;
        user.JobTitle = form.JobTitle;

        if (imagePath != null)
        {
            user.UserImage = imagePath;
        }
       
        // Handle address
        user.Address ??= new UserAddressEntity { UserId = user.Id };
        user.Address.StreetName = form.StreetName ?? string.Empty;
        user.Address.PostalCode = form.PostalCode ?? string.Empty;
        user.Address.City = form.City ?? string.Empty;

        // **Update User Role by chat gpt
        var existingRoles = await _userManager.GetRolesAsync(user);
        var newRole = form.Role;

        // Remove existing roles
        await _userManager.RemoveFromRolesAsync(user, existingRoles);

        // Add the new role
        var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);
        if (!addRoleResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Failed to update user role.");
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }
        // Save changes
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Problem("Unable to edit data");

    }

    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrEmpty(term))
            return Json(new List<object>());

        var users = await _context.Users
            .Where(x => x.FirstName!.Contains(term) || x.LastName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new
            {
                x.Id,
                MemberImage = x.UserImage,
                //Image = string.IsNullOrEmpty(x.UserImage) ? "" : "/uploads/members/" + x.UserImage,
                FullName = x.FirstName + " " + x.LastName
            })
            .ToListAsync();

        return Json(users);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        return Problem("Unable to delete the user.");
    }
}
