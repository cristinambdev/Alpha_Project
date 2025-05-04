using Business.Services;
using Data.Contexts;
using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;


namespace Presentation.Controllers;

public class MiniProjectsController(AppDbContext context, IMiniProjectService miniProjectService, IWebHostEnvironment env, IStatusService statusService, ILogger<IMiniProjectRepository> logger) : Controller
{
    private readonly AppDbContext _context = context;
    private readonly IMiniProjectService _miniProjectService = miniProjectService;
    private readonly IWebHostEnvironment _env = env;
    private readonly IStatusService _statusService = statusService;
    private readonly ILogger<IMiniProjectRepository> _logger = logger;



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


    // Helper method to calculate TimeLeft (you might already have this)
    private string GetTimeLeft(DateTime endDate)
    {
        var timeLeft = endDate - DateTime.Now;
        if (timeLeft.TotalDays > 1)
        {
            return $"{timeLeft.Days} days left";
        }
        else if (timeLeft.TotalHours > 1)
        {
            return $"{timeLeft.Hours} hours left";
        }
        else if (timeLeft.TotalMinutes > 1)
        {
            return $"{timeLeft.Minutes} minutes left";
        }
        else if (timeLeft.TotalSeconds >= 0)
        {
            return $"{timeLeft.Seconds} seconds left";
        }
        else
        {
            return "Overdue";
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddMiniProject(AddMiniProjectViewModel form)
    {
        Console.WriteLine($"Received Budget in AddMiniProject: {form.Budget}");

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
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "miniprojects");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ProjectImage.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.ProjectImage.CopyToAsync(stream);
                }

                imagePath = $"/uploads/miniprojects/{fileName}";
            }
            var formData = new AddMiniProjectFormData
            {
                Title = form.Title,
                Description = form.Description,
                ClientName = form.ClientName,
                StartDate = form.StartDate!.Value,
                EndDate = form.EndDate!.Value,
                Budget = form.Budget,
                ProjectImage = imagePath,
                StatusId = form.StatusId,
            };


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
    [HttpGet]
    public async Task<IActionResult> EditMiniProject(string id)
    {
        var project = await _context.MiniProjects
       .AsNoTracking() // Don't track original suggested by chat gpt because a different Id was creating when opening edit form
       .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        // Fetch status options for the dropdown
        var statusResult = await _statusService.GetStatusesAsync();
        var statusOptions = statusResult.Succeeded && statusResult.Result != null
            ? statusResult.Result.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StatusName
            }).ToList()
            : new List<SelectListItem>();  // Fallback if statuses are not available

        var viewModel = new EditMiniProjectViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ClientName = project.ClientName!,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            StatusId = project.StatusId,
            ExistingImageUrl = project.ProjectImage,
            StatusOptions = statusOptions  // Pass the status options here
        };
        _logger.LogInformation("View model created with ID: {ViewModelId}", viewModel.Id);

        return View(viewModel);

    }

    [HttpPost]
    public async Task<IActionResult> EditMiniProject(EditMiniProjectViewModel model, string id)
    {
        Console.WriteLine("EditMiniProject POST called");
        Console.WriteLine($"ID from URL: {id}");
        Console.WriteLine($"ID from model: {model.Id}");

        if (model.Id != id) return BadRequest("ID mismatch");

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

        // Handle file upload 

        string? imagePath = null;
        if (model.ProjectImage != null)
        {
            // Handle image upload
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProjectImage.FileName)}";
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "miniprojects");
            Directory.CreateDirectory(uploadPath);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProjectImage.CopyToAsync(stream);
            }

            imagePath = $"/uploads/miniprojects/{fileName}";
        }

        // Create a form data object expected by the service method
        var formData = new EditMiniProjectFormData
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            ClientName = model.ClientName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            StatusId = model.StatusId,
            ExistingImageUrl = model.ExistingImageUrl
            //ProjectImage = model.ProjectImage,
            //ExistingImageUrl = model.ExistingImageUrl
        };
        if (imagePath != null)
        {
            formData.ExistingImageUrl = imagePath;
        }
        // Call the service method 
        var result = await _miniProjectService.UpdateMiniProjectAsync(formData);
        await _context.SaveChangesAsync();

           return Ok(new { success = true });
        

        // If update failed, return error
    }

    //suggested by chat gpt as there was a mismatch between Ids in the editminiprojectsform
    [HttpGet("ValidateProject/{id}")]
    public async Task<IActionResult> ValidateProject(string id)
    {
        var exists = await _context.MiniProjects.AnyAsync(p => p.Id == id);
        return exists ? Ok() : NotFound();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> GetProjects()
    {
        return Json(await _context.MiniProjects.ToListAsync());
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
