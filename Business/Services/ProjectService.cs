using Domain.Models;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extentions;
using Azure;
using System.Runtime.Intrinsics.X86;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService, IClientService clientService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;



    //public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    //{
    //    if (formData == null)
    //        return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

    //    var client = await _clientService.GetClientByNameAsync(formData.ClientName);

    //    if (client.Result == null)
    //    {
    //        // Create client if it doesn't exist
    //        var createClientResult = await _clientService.CreateClientAsync(new AddClientFormData
    //        {
    //            ClientName = formData.ClientName
    //        });

    //        if (!createClientResult.Succeeded)
    //           return new ProjectResult { Succeeded = false, StatusCode = createClientResult.StatusCode, Error = createClientResult.Error};

    //        // Get the newly created client
    //        client= await _clientService.GetClientByNameAsync(formData.ClientName);
    //    }

    //    // Map form data to ProjectEntity
    //    var projectEntity = formData.MapTo<ProjectEntity>();

    //    //// Set ClientId
    //    var clientEntity = client.Result!.FirstOrDefault();
    //    projectEntity.ClientId = clientEntity?.Id!;

    //    // Set default StatusId
    //    var statusResult = await _statusService.GetStatusByIdAsync(1);
    //    projectEntity.StatusId = statusResult.Result!.Id;

    //    // 💾 Save project
    //    var result = await _projectRepository.AddAsync(projectEntity);

    //    return result.Succeeded
    //        ? new ProjectResult { Succeeded = true, StatusCode = 201 }
    //        : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

    //}
    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        // Add logging here to debug
        Console.WriteLine($"Creating project: {formData.ProjectName} for client: {formData.ClientName}");

        var client = await _clientService.GetClientByNameAsync(formData.ClientName);

        if (client.Result == null || !client.Result.Any())
        {
            // Create client if it doesn't exist
            var createClientResult = await _clientService.CreateClientAsync(new AddClientFormData
            {
                ClientName = formData.ClientName
            });

            if (!createClientResult.Succeeded)
                return new ProjectResult { Succeeded = false, StatusCode = createClientResult.StatusCode, Error = createClientResult.Error };

            // Get the newly created client
            client = await _clientService.GetClientByNameAsync(formData.ClientName);

            if (client.Result == null || !client.Result.Any())
            {
                return new ProjectResult { Succeeded = false, StatusCode = 500, Error = "Failed to create client." };
            }
        }

        // Map form data to ProjectEntity
        var projectEntity = formData.MapTo<ProjectEntity>();

        // Set ClientId
        var clientEntity = client.Result!.FirstOrDefault();
        if (clientEntity == null)
        {
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Client not found." };
        }

        projectEntity.ClientId = clientEntity.Id!;

        // Set default StatusId
        var statusResult = await _statusService.GetStatusByIdAsync(1);
        if (statusResult.Result == null)
        {
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Status not found." };
        }

        projectEntity.StatusId = statusResult.Result!.Id;

        // 💾 Save project
        var result = await _projectRepository.AddAsync(projectEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllAsync
            (orderByDescending: true,
            sortBy: s => s.Created,
            where: null,
            include => include.User,
            include => include.Status,
            include => include.Client
            );

        //return response.MapTo<ProjectResult<IEnumerable<Project>>>();

        return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
            where: x => x.Id == id, //how I want to filter it
            include => include.User, //what I want to include
            include => include.Status,
            include => include.Client
            );

        return response.Succeeded
            ? new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<Project> { Succeeded = false, StatusCode = 403, Error = $"Project '{id}' was not found" };

        //return response.MapTo<ProjectResult<Project>>();
    }

    public async Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData)
    {
        try
        {

            var existingProject = await _projectRepository.GetAsync(x => x.Id == formData.Id);
            if (existingProject == null)
            {
                throw new Exception("Project not found");
            }

            var projectEntity = formData.MapTo<ProjectEntity>();

            var result = await _projectRepository.UpdateAsync(projectEntity);
            return new ProjectResult { Succeeded = true, StatusCode = 201 };
        }
        catch
        {
            return new ProjectResult { Succeeded = false, StatusCode = 500 };
        }
    }

    // DELETE missing

}
