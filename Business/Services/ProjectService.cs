using Domain.Models;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extentions;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Exchange.WebServices.Data;


namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService, IClientService clientService, ILogger<ProjectService> logger) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;
    private readonly ILogger<ProjectService> _logger = logger; // Inject this



    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        // Fetch the client by name
        var clientResult = await _clientService.GetClientByNameAsync(formData.ClientName);
        var existingClient = clientResult.Result?.FirstOrDefault();

        Data.Entities.ClientEntity? clientEntity = null; // by chat gpt Initialize as null

        // If a client with the given name exists, map it to the entity
        if (existingClient != null)
        {
            clientEntity = existingClient.MapTo<Data.Entities.ClientEntity>();
        }
        // If no client found, create a new one

        else
        {
            var createClientResult = await _clientService.CreateClientAsync(new AddClientFormData
            {
                ClientName = formData.ClientName
            });

            _logger.LogInformation($"CreateClientResult Succeeded: {createClientResult.Succeeded}, StatusCode: {createClientResult.StatusCode}, Error: {createClientResult.Error}");

            if (!createClientResult.Succeeded)
            {
                return new ProjectResult { Succeeded = false, StatusCode = createClientResult.StatusCode, Error = createClientResult.Error };
            }

            var newClientResult = await _clientService.GetClientByNameAsync(formData.ClientName);
            _logger.LogInformation($"NewClientResult Succeeded: {newClientResult.Succeeded}, StatusCode: {newClientResult.StatusCode}");
            var newClient = newClientResult.Result?.FirstOrDefault();
            _logger.LogInformation($"NewClient: {(newClient != null ? newClient.ClientName : "null")}");

            if (newClient != null)
            {
                var mappedClientEntity = newClient.MapTo<Data.Entities.ClientEntity>();
                _logger.LogInformation($"MappedClientEntity: {(mappedClientEntity != null ? mappedClientEntity.ClientName : "null")}");
                clientEntity = mappedClientEntity;
            }

            if (clientEntity == null)
            {
                _logger.LogError("Failed to retrieve or map the newly created client.");
                return new ProjectResult { Succeeded = false, StatusCode = 500, Error = "Failed to retrieve or map the newly created client." };
            }
        }

        // If we couldn't find or create and map a client, return an error
        if (clientEntity == null)
        {
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Client not found or could not be created/mapped." };
        }

        // Map form data to ProjectEntity
        var projectEntity = formData.MapTo<ProjectEntity>();

        // Create ProjectClientEntity to associate client with project
        var projectClientEntity = new ProjectClientEntity
        {
            ProjectId = projectEntity.Id,
            ClientId = clientEntity.Id,
            Project = projectEntity,
            Client = clientEntity // Now using the correct entity type
        };
        // Add the association to the ProjectEntity's ProjectClients collection
        projectEntity.ProjectClients.Add(projectClientEntity);

        // Set default StatusId
        var statusResult = await _statusService.GetStatusByIdAsync(1);
        if (statusResult.Result == null)
        {
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Status not found." };
        }

        projectEntity.StatusId = statusResult.Result!.Id;

        // Save project
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
            include => include.ProjectUsers,
            include => include.Status,
            include => include.ProjectClients
            );

        //return response.MapTo<ProjectResult<IEnumerable<Project>>>();

        return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }



    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
            where: x => x.Id == id, //how I want to filter it
            include => include.ProjectUsers, //what I want to include
            include => include.Status,
            include => include.ProjectClients
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
