
using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace Business.Services;

public interface IMiniProjectService
{
    Task<MiniProjectResult> CreateMiniProjectAsync(AddMiniProjectFormData form);
    Task<MiniProjectResult<MiniProject>> GetMiniProjectAsync(string id);    
    Task<MiniProjectResult<IEnumerable<MiniProject>>> GetAllMiniProjectsAsync();
    Task<MiniProjectResult> DeleteMiniProjectAsync(string id);
    Task<List<SelectListItem>> GetStatusOptionsAsync();
    Task<MiniProjectResult<MiniProject>> GetMiniProjectByTitleAsync(string title);
    Task<MiniProjectResult> UpdateMiniProjectAsync(EditMiniProjectFormData formData);
    Task<MiniProjectEntity> GetMiniProjectByIdAsync(string id);
}

public class MiniProjectService(AppDbContext context, IMiniProjectRepository miniprojectRepository, ILogger<IMiniProjectRepository> logger, IWebHostEnvironment env) : IMiniProjectService
{
    private readonly IMiniProjectRepository _miniprojectRepository = miniprojectRepository;
    private readonly AppDbContext _context = context;
    private readonly ILogger<IMiniProjectRepository> _logger = logger;
    private readonly IWebHostEnvironment _env = env;



    public async Task<MiniProjectResult> CreateMiniProjectAsync(AddMiniProjectFormData form)
    {

        if (form == null)
        {
            return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        }

        try
        {
            
            var miniprojectEntity = form.MapTo<MiniProjectEntity>();
            
            if (miniprojectEntity == null)
            {
                return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Failed to create project." };

            }

            // Save Project
            var result = await _miniprojectRepository.AddAsync(miniprojectEntity);


            if (result.Succeeded) // Accessing the boolean value inside the RepositoryResult
            {

                return new MiniProjectResult { Succeeded = true, StatusCode = 201 };
            }
            else
            {

                return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Failed to create project." };
            }

        }
        catch 
        {
            return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Failed to create project." };
        }
    }
    // Chat GPT Method to get status options for the dropdown
    public async Task<List<SelectListItem>> GetStatusOptionsAsync()
    {
        var statusList = await _context.Statuses
                                       .Select(s => new SelectListItem
                                       {
                                           Value = s.Id.ToString(),
                                           Text = s.StatusName
                                       })
                                       .ToListAsync();

        return statusList;
    }

    public async Task<MiniProjectResult<IEnumerable<MiniProject>>> GetAllMiniProjectsAsync()
    {
        var response = await _miniprojectRepository.GetAllAsync
        (orderByDescending: true,
        sortBy: s => s.EndDate!,
        where: null,
        include => include.Status!
        );

        return response.MapTo<MiniProjectResult<IEnumerable<MiniProject>>>();

    }

    public async Task<MiniProjectResult<MiniProject>> GetMiniProjectAsync(string id)
    {
        var response = await _miniprojectRepository.GetAsync
        (
        where: x => x.Id == id,
        include => include.Status!
        );

        return response.Succeeded
            ? new MiniProjectResult<MiniProject> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new MiniProjectResult<MiniProject> { Succeeded = false, StatusCode = 403, Error = $"Project '{id}' was not found" };

    }

    public async Task<MiniProjectResult<MiniProject>> GetMiniProjectByTitleAsync(string title)
    {
        var projectEntity = await _context.MiniProjects
            .FirstOrDefaultAsync(p => p.Title == title);
        if (projectEntity == null)
            return new MiniProjectResult<MiniProject> { Succeeded = false, StatusCode = 404 };

        var project = projectEntity?.MapTo<MiniProject>();

        return new MiniProjectResult<MiniProject> { Succeeded = true, StatusCode = 200 };
    }
    public async Task<MiniProjectEntity> GetMiniProjectByIdAsync(string id)
    {

        //return project;
        var project = await _context.MiniProjects.FirstOrDefaultAsync(p => p.Id == id);
        return project ?? throw new KeyNotFoundException($"MiniProject with ID '{id}' was not found.");

    }

  

    public async Task<MiniProjectResult> UpdateMiniProjectAsync(EditMiniProjectFormData form)
    {
        try
        {
            if (form== null)
                return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Form data can't be null" };

            // Debug output to help diagnose issues
            Debug.WriteLine($"UpdateMiniProjectAsync called for project ID: {form.Id}");


            var existingProject = await _miniprojectRepository.GetAsync(x => x.Id == form.Id);
            if (existingProject.Result == null)
            {
                Debug.WriteLine($"Project with ID '{form.Id}' not found");
                return new MiniProjectResult { Succeeded = false, StatusCode = 404, Error = "Project not found" };
            }

            var projectEntity = form.MapTo<MiniProjectEntity>();
            Debug.WriteLine($"Mapped to entity with ID: {projectEntity.Id}, Name: {projectEntity.Title}");


            var updateResult = await _miniprojectRepository.UpdateAsync(projectEntity);
            Debug.WriteLine($"Update result: Success={updateResult.Succeeded}, Status={updateResult.StatusCode}, Error={updateResult.Error}");

            return new MiniProjectResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error };

        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating client: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");

            return new MiniProjectResult { Succeeded = false, StatusCode = 500 };
        }
       
    }

    public async Task<MiniProjectResult> DeleteMiniProjectAsync(string id)
    {

        try
        {
            var existingProject = await _miniprojectRepository.GetAsync(x => x.Id == id);
            if (!existingProject.Succeeded || existingProject == null)
                return new MiniProjectResult { Succeeded = false, StatusCode = 500, Error = "Project not found" };

            var entityToDelete = existingProject.Result?.MapTo<MiniProjectEntity>();

            var result = await _miniprojectRepository.DeleteAsync(entityToDelete!);


            if (result.Succeeded)
                return new MiniProjectResult { Succeeded = true, StatusCode = 200 };

            return new MiniProjectResult { Succeeded = false, StatusCode = 400, Error = "Failed to delete project" };

        }
        catch
        {
            return new MiniProjectResult { Succeeded = false, StatusCode = 500, Error = "Internal server error" };

        }

    }
}

