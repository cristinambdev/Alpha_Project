using Business.Services;
using Data.Contexts;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.SqlServer.Server;
using Presentation.Models;
using System.Security.Claims;
using System.Text.Json;

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
    public async Task<IActionResult> Index()
    {

        var projectResult = await _projectService.GetProjectsAsync();
        var clients = await _clientService.GetClientsAsync();
        var users = await _userService.GetUsersAsync();

        var viewModel = new ProjectsViewModel
        {
            Projects = projectResult.Result?.Select(project => new ProjectViewModel
            {
                Id = project.Id,
                ClientImage = project.Client?.Image ?? "", // Handle null Client
                ProjectName = project.ProjectName,
                ClientName = project.Client?.ClientName ?? "No client assigned", // Handle null Client
                Description = project.Description ?? "No description", // Handle null Description
                TimeLeft = GetTimeLeft(project.EndDate),
                Users = project.User != null
                    ? new List<string> { project.User.UserImage ?? "default-user-image" }
                    : new List<string> { "No user assigned" } // Handle null User
            }) ?? []
        };

        return View(viewModel);
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
    public async Task<IActionResult> AddProject(AddProjectViewModel form, string SelectedUserIds)
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
        //var existingMembers = await _context.Users
        //    .Where(m => m.Projects.ProjectId == form.Id)
        //    .ToListAsync();

        //_userService.GetUsersAsync.RemoveRange(existingMembers):
        //    if(!string.IsNullOrEmpty(SelectedUserIds))
        //    {
        //    var userIds = JsonSerializer.Deserelaize<List<int>>(SelectedUserIds)
        //        if(userIds != null) 
        //        {
        //        foreach(var userId in userIds)
        //            {
        //            _context.Users.Add(new UserEntity { ProjectId = form.Id, UserId = userId})
        //            }
        //        }
        //    }

        var addProjectFormData = form.MapTo<AddProjectFormData>();

        //upload image handling
        if (form.ClientImage != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "clients");
            Directory.CreateDirectory(uploadFolder);

            // By Chat GPT: restructuring of code for image filename and storing path
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage!.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            //var filePath = Path.Combine(uploadFolder, $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.ClientImage.CopyToAsync(stream);
            }

            // By Chat GPT: Set image path to display later
            addProjectFormData.Image = $"/uploads/clients/{fileName}";
        }
        // If using authentication, set the UserId from the current user
        if (string.IsNullOrEmpty(form.UserId))
        {
            form.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

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
        //var result = await _projectService.UpdateProjectAsync(editProjectFormData);
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
    [HttpGet]


    [HttpDelete]
    public IActionResult Delete(string id)
    {
        return Json(new { });
    }
}
