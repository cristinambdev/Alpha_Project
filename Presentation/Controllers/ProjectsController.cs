using Business.Services;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;

    [HttpGet]
    [Route("projects")]
    public async Task<IActionResult> Index()
    {
        var projectResult = await _projectService.GetProjectsAsync();
        return View(projectResult);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectFormData form)
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

        //var addProjectFormData = form.MapTo<AddProjectFormData>();

        // Make sure UserId is set - this might be missing in your form
        // If you're using authentication, set the UserId from the current user
        //if (string.IsNullOrEmpty(addProjectFormData.UserId))
        //{
        //    addProjectFormData.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        //}

        var result = await _projectService.CreateProjectAsync(form);

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
        //var editProjectFormData = form.MapTo<EditProjectFormData>();
        //var result = await _projectService.UpdateProjectAsync(editProjectFormData);
        var result = await _projectService.UpdateProjectAsync(form);
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
