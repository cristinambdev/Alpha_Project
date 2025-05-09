using Business.Services;
using Data.Contexts;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;


namespace Presentation.Controllers;

public class MiniProjectsController(AppDbContext context, IMiniProjectService miniProjectService, IStatusService statusService, IImageService imageService) : Controller
{
    private readonly AppDbContext _context = context;
    private readonly IMiniProjectService _miniProjectService = miniProjectService;
    private readonly IStatusService _statusService = statusService;
    private readonly IImageService _imageService = imageService;


    public async Task<IActionResult> Index()
    {
        var allProjects = await _context.MiniProjects
       .Include(x => x.Status)
       .ToListAsync();
        var miniprojectViewModels = allProjects.Select(p => new MiniProjectViewModel
        {
            Title = p.Title,
            Description = p.Description! ?? "",
            ClientName = p.ClientName!,
            ProjectImage = p.ProjectImage,
            StatusId = p.StatusId,
            StatusName = p.Status?.StatusName ?? "Unknown",
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Budget = p.Budget // Map the Budget property
        }).ToList();

        // Get status options for use in modal forms
        var statusResult = await _statusService.GetStatusesAsync();
        var statusOptions = statusResult.Succeeded && statusResult.Result != null
            ? statusResult.Result.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StatusName
            }).ToList()
            : new List<SelectListItem>();

        var viewModel = new MiniProjectsViewModel
        {
            MiniProjects = miniprojectViewModels,
            StatusOptions = statusOptions
        };

        return View(viewModel);
    }


  
    [HttpPost]
    public async Task<IActionResult> AddMiniProject(AddMiniProjectViewModel form)
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
                return BadRequest(new { success = false, errors });
            }

            var existingProject = await _miniProjectService.GetMiniProjectByTitleAsync(form.Title);
            if (existingProject.Succeeded && existingProject.Result != null)
            {
                ModelState.AddModelError("Title", "A project with the same title already exists.");
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                    );
                return BadRequest(new { success = false, errors });
            }
            //  Handle the file upload
            string? imagePath = null;
            if (form.ProjectImage != null)
            {
                imagePath = await _imageService.SaveImageAsync(form.ProjectImage, "miniprojects");

            }
            //Mapping
            var formData = form.MapTo<AddMiniProjectFormData>();
            formData.ProjectImage = imagePath;

            var result = await _miniProjectService.CreateMiniProjectAsync(formData);
            if (!result.Succeeded)
            {

                return BadRequest(new { Succeeded = false, StatusCode = 400 });
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Succeeded = false, error = ex.Message });
        }

    }
    [HttpGet] //suggested by Chat gpt to populate the form
    public async Task<IActionResult> EditMiniProject(string id)
    {
        var miniProject = await _miniProjectService.GetMiniProjectByIdAsync(id);
        var statuses = await _statusService.GetStatusesAsync();

        var viewModel = new EditMiniProjectViewModel
        {
            Id = miniProject.Id,
            Title = miniProject.Title,
            Description = miniProject.Description,
            ClientName = miniProject.ClientName!,
            StartDate = miniProject.StartDate,
            EndDate = miniProject.EndDate,
            Budget = miniProject.Budget,
            StatusId = miniProject.StatusId,
            StatusOptions = statuses.Result?.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StatusName,
            })
        };

        return View(viewModel);

    }

    [HttpPost]
    public async Task<IActionResult> EditMiniProject(EditMiniProjectViewModel model, string id)
    {
       

        if (!ModelState.IsValid)
        {
            // Get status list for dropdown if validation fails
            var status = await _statusService.GetStatusesAsync();
            if (status.Succeeded && status.Result != null)
            {
                model.StatusOptions = status.Result.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.StatusName
                }).ToList();
            }

            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return BadRequest(new { errors });
        }

        

        // Create a form data object expected by the service method
        //var formData = model.MapTo<EditMiniProjectFormData>();
        Console.WriteLine("Looking for project ID: " + model.Id);

        var existing = await _miniProjectService.GetMiniProjectByTitleAsync(model.Title);
        if (existing == null)
            return NotFound();
        //  Handle the file upload
        string? imagePath = null;
        if (model.ProjectImage != null)
        {
            imagePath = await _imageService.SaveImageAsync(model.ProjectImage, "miniprojects");

        }
        var formData = new EditMiniProjectFormData
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            ClientName = model.ClientName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ProjectImage = imagePath,
            StatusId = model.StatusId,

        };
        // Call the service method 
        var result = await _miniProjectService.UpdateMiniProjectAsync(formData);
        await _context.SaveChangesAsync();

           return Ok(new { success = true });
            }


    [HttpPost]
    public async Task<IActionResult> DeleteMiniProject(string id)
    {
        var miniproject = await _miniProjectService.GetMiniProjectAsync(id);
        if (miniproject == null)
            return NotFound($"MiniProject with Title {id} not found.");
        var result = await _miniProjectService.DeleteMiniProjectAsync(id);
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "MiniProject deleted successfully!" }); // Return JSON for AJAX

        }

        return BadRequest(new { success = false, message = "Delete failed", errors = new[] { "Specific error" } });
    }
}
