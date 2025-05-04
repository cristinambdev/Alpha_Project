using Business.Services;
using Data.Contexts;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

public class ProjectsController(IProjectService projectService, IWebHostEnvironment env, IClientService clientService, IUserService userService, AppDbContext context) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IUserService _userService = userService;
    private readonly IWebHostEnvironment _env = env;
    private readonly AppDbContext _context = context;


    [HttpGet]
    [Route("admin/projects")]
    public async Task<IActionResult> Index(string id)
    {
        var allProjects = await _context.Projects
            .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.User)
            .Include(x => x.ProjectClients)
                .ThenInclude(x => x.Client)
            .ToListAsync();

        // Fetch users and clients
        var users = await _context.Users.ToListAsync();
        var clients = await _context.Clients.ToListAsync();

        // Map projects to ProjectViewModel with help of Chat GPT
        var projectViewModels = allProjects.Select(p => new ProjectViewModel
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Description = p.Description!,
            TimeLeft = GetTimeLeft(p.EndDate),
            Clients = p.ProjectClients.Select(pc => new ClientViewModel
            {
                Id = pc.Client.Id,
                ClientName = pc.Client.ClientName,
                Image = pc.Client.Image
            }).ToList(),
            Users = p.ProjectUsers.Select(u => new UserViewModel
            {
                Id = u.User.Id,
                FirstName = u.User.FirstName,
                LastName = u.User.LastName,
                UserImage = u.User.UserImage
            }).ToList()
        }).ToList();

        var model = new ProjectsViewModel
        {
            Projects = projectViewModels,
            Users = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserImage = u.UserImage
            }).ToList(),
            Clients = clients.Select(c => new ClientViewModel
            {
                Id = c.Id,
                ClientName = c.ClientName,
                Image = c.Image
            }).ToList()
        };

        return View(model); 
    }


    private string GetTimeLeft(DateTime? endDate)//created by chat GPT to calculate time left
    {
        if (endDate == null)
            return "No deadline";

        var daysLeft = (endDate.Value - DateTime.Now).Days;

        return daysLeft switch
        {
            < 0 => "Overdue",
            0 => "Today",
            1 => "1 day left",
            <= 7 => $"{daysLeft} days left",
            _ => $"{(int)Math.Ceiling(daysLeft / 7.0)} weeks left"
        };
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel form)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToList()
                    );
                return BadRequest(new { success = false, errors });
            }
            // Chat gpt -Set default UserId if not provided in the form
            // Ensure Id is generated if missing
            if (string.IsNullOrEmpty(form.Id))
            {
                form.Id = Guid.NewGuid().ToString();
            }

            // Ensure UserId is populated, either from form or current user
            if (string.IsNullOrEmpty(form.UserId))
            {
                form.UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            }

            if (string.IsNullOrEmpty(form.UserId))
            {
                ModelState.AddModelError("UserId", "UserId is required.");
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            //upload image handling
            string? imagePath = null; // suggested by chat GPT to store the image value
            if (form.ClientImage != null)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "clients");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.ClientImage.CopyToAsync(stream);
                }

                imagePath = $"/uploads/clients/{fileName}";
            }


            // Map the view model to form data for the service
            var addProjectFormData = form.MapTo<AddProjectFormData>();

            // Manually set any properties that need special handling
            var formDataType = typeof(AddProjectFormData);
            if (formDataType.GetProperty("Image") != null)
            {
                formDataType.GetProperty("Image")!.SetValue(addProjectFormData, imagePath);
            }
            // Call the service to create the project
            var result = await _projectService.CreateProjectAsync(addProjectFormData);


            if (result.Succeeded)
            {
                return Ok(new { success = true });
            }
            else
            {
                return Problem($"Unable to submit data: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }

    }

    [HttpPut]
    public async Task<IActionResult> EditProject(EditProjectFormData form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToList()
                );
            return BadRequest(new { success = false, errors });
        }
        var editProjectFormData = form.MapTo<EditProjectFormData>();

        string? imagePath = null;

        if (form.ClientImage != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "clients");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.ClientImage.CopyToAsync(stream);
            }

            imagePath = $"/uploads/clients/{fileName}";
        }
        var result = await _projectService.UpdateProjectAsync(editProjectFormData);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }
        else
        {
            return Problem("Unable to edit data");
        }

    }


    [HttpDelete]
    public IActionResult Delete(string id)
    {
        return Json(new { });
    }

}
