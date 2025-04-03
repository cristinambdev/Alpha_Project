using Business.Services;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

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
    public async Task<IActionResult> AddProject(AddProjectViewModel form)
    {
        var addProjectFormData = form.MapTo<AddProjectFormData>();

        var result = await _projectService.CreateProjectAsync(addProjectFormData);

        return Json(new { });
    }

    [HttpPut]
    public IActionResult EditProject(EditProjectViewModel form)
    {
        return Json(new { });
    }

    [HttpDelete]
    public IActionResult Delete(string id)
    {
        return Json(new { });
    }
}
