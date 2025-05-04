using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Models;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult> CreateClientAsync(AddClientFormData formData);
    Task<ClientResult> GetClientsAsync();

    Task<ClientResult> GetClientByIdAsync(string id);
    Task<ClientResult> UpdateClientAsync(EditClientFormData formData);
    Task<ClientResult> GetClientByNameAsync(string name);
    Task<ClientResult> DeleteClientAsync(string id);
    //Task<ClientResult> DeleteClientAsync(string id);
}

public class ClientService(IClientRepository clientRepository, ILogger<ClientService> logger, AppDbContext context) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly ILogger<ClientService> _logger = logger; // Inject logger
    private readonly AppDbContext _context= context;


    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>(); // dynamic mapping

    }

    public async Task<ClientResult> GetClientByIdAsync(string id)
    {
        var repositoryResult = await _clientRepository.GetAsync(x => x.Id == id);

        if (repositoryResult.Succeeded && repositoryResult.Result != null)
        {
            // Directly map the Client (TModel from repository) to ClientResult
            return repositoryResult.Result.MapTo<ClientResult>();
        }
        else
        {
            // Log the error message if it exists
            if (!string.IsNullOrEmpty(repositoryResult.Error))
            {
                _logger.LogError($"Error retrieving client with ID {id}: {repositoryResult.Error}");
            }

            return new ClientResult { Succeeded = false, Error = "" };
        }
    }



    public async Task<ClientResult> GetClientByNameAsync(string name)
    {
        var result = await _clientRepository.GetAsync(x => x.ClientName == name);
        return result.MapTo<ClientResult>();
    }

    public async Task<ClientResult> CreateClientAsync(AddClientFormData formData)
    {
        if (formData == null)
            return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Form data can't be null" };

        var existsResult = await _clientRepository.ExistsAsync(x => x.ClientName == formData.ClientName);
        if (existsResult.Succeeded)
            return new ClientResult { Succeeded = false, StatusCode = 409, Error = "Client with same name already exists." };



        try
        {
            var clientEntity = formData.MapTo<ClientEntity>();
            clientEntity.Id = Guid.NewGuid().ToString(); //suggested by chat gpt
                                                         //var result = await _clientRepository.AddAsync(clientEntity);

            //var clientEntity = new ClientEntity // suggested by Chat Gpt as the dynamic mapping didn't work 
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    ClientName = formData.ClientName

            //};
            await _clientRepository.AddAsync(clientEntity);

            return new ClientResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ClientResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<ClientResult> UpdateClientAsync(EditClientFormData formData)
    {
        try
        {
            if (formData == null)
                return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Form data can't be null" };

            // Debug output to help diagnose issues
            Debug.WriteLine($"UpdateClientAsync called for client ID: {formData.Id}");


            var existingClient = await _clientRepository.GetAsync(x => x.Id == formData.Id);
            if (existingClient.Result == null)
            {
                Debug.WriteLine($"Client with ID '{formData.Id}' not found");
                return new ClientResult { Succeeded = false, StatusCode = 404, Error = "Client not found" };
            }

            var clientEntity = formData.MapTo<ClientEntity>();
            Debug.WriteLine($"Mapped to entity with ID: {clientEntity.Id}, Name: {clientEntity.ClientName}");


            var updateResult = await _clientRepository.UpdateAsync(clientEntity);
            Debug.WriteLine($"Update result: Success={updateResult.Succeeded}, Status={updateResult.StatusCode}, Error={updateResult.Error}");

            return new ClientResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error };

        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating client: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");

            return new ClientResult { Succeeded = false, StatusCode = 500 };
        }

    }

    //public async Task<ClientResult> DeleteClientAsync(string id)
    //{


    //    var existingClient = await _clientRepository.GetAsync(x => x.Id == id);
    //    if (existingClient.Result == null)
    //    {
    //        throw new Exception("Customer not found");
    //    }
    //    var clientEntity = existingClient.Result.MapTo<ClientEntity>();
    //    await _clientRepository.DeleteAsync(clientEntity);

    //    return new ClientResult { Succeeded = true, Error = "Client deleted successfully" };
    //}

    public async Task<ClientResult> DeleteClientAsync(string id)
    {
        try
        {
            var existingCustomerResult = await _clientRepository.GetAsync(x => x.Id == id);

            if (!existingCustomerResult.Succeeded || existingCustomerResult.Result == null)
            {
                return new ClientResult { Succeeded = false, Error = "Customer not found." } ;
            }

            var clientToDelete = existingCustomerResult.Result;
            var clientEntityToDelete = new ClientEntity
            {
                Id = clientToDelete.Id,
                ClientName = clientToDelete.ClientName,
                Email = clientToDelete.Email,
                Phone = clientToDelete.Phone,
                StreetName = clientToDelete.StreetName,
                PostalCode = clientToDelete.PostalCode,
                City = clientToDelete.City,
                Date = clientToDelete.Date,
                Status = clientToDelete.Status,
                Image = clientToDelete.Image
                // Map other properties as needed
            };

            var deletionResult = await _clientRepository.DeleteAsync(clientEntityToDelete);

            if (deletionResult.Succeeded)
            {
                return new ClientResult { Succeeded = true, StatusCode = 200 };
            }
            else
            {
                return new ClientResult { Succeeded = false,  Error = "Problems deleting client" }; // Ensure Errors is a list
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new ClientResult { Succeeded = false, Error = "An error occurred during deletion." };
        }
    }
}


    //public async Task<ClientResult> DeleteClientAsync(string id)
    //{
    //    var existingClientResult = await _clientRepository.GetAsync(x => x.Id == id);

    //    if (!existingClientResult.Succeeded || existingClientResult.Result == null)
    //    {
    //        return new ClientResult { Succeeded = false, Error = "Client not found."  };
    //    }

    //    var clientToDelete = existingClientResult.Result;
    //    var clientEntityToDelete = new ClientEntity
    //    {
    //        Id = clientToDelete.Id,
    //        ClientName = clientToDelete.ClientName,
    //        Email = clientToDelete.Email,
    //        Phone = clientToDelete.Phone,
    //        StreetName = clientToDelete.StreetName,
    //        PostalCode = clientToDelete.PostalCode,
    //        City = clientToDelete.City,
    //        Date = clientToDelete.Date,
    //        Status = clientToDelete.Status,
    //        Image = clientToDelete.Image
    //        // Map other properties as needed
    //    };


    //    var deletionResult = await _clientRepository.DeleteAsync(clientEntityToDelete);

    //    if (deletionResult.Succeeded)
    //    {
    //        return new ClientResult { Succeeded = true, StatusCode = 200};
    //    }
    //    else
    //    {
    //        return new ClientResult { Succeeded = false, Error = "Could not delete" }; // Use the Error property from RepositoryResult
    //    }
    //}


