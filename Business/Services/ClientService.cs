using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.SqlServer.Server;
using System.Diagnostics;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult> CreateClientAsync(AddClientFormData formData);
    Task<ClientResult> GetClientsAsync();

    Task<ClientResult> GetClientByIdAsync(string id);
    Task<ClientResult> UpdateClientAsync(EditClientFormData formData);
    Task<ClientResult> GetClientByNameAsync(string name);
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>(); // dynamic mapping

    }

    public async Task<ClientResult> GetClientByIdAsync(string id)
    {
        var result = await _clientRepository.GetAsync(x => x.Id == id);
        return result.MapTo<ClientResult>();
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

    public async  Task<ClientResult> UpdateClientAsync(EditClientFormData formData)
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
                return new ClientResult { Succeeded = false,  StatusCode = 404, Error = "Client not found"};
            }

            var clientEntity = formData.MapTo<ClientEntity>();
            Debug.WriteLine($"Mapped to entity with ID: {clientEntity.Id}, Name: {clientEntity.ClientName}");


            var updateResult = await _clientRepository.UpdateAsync(clientEntity);
            Debug.WriteLine($"Update result: Success={updateResult.Succeeded}, Status={updateResult.StatusCode}, Error={updateResult.Error}");

            return new ClientResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error};
           
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating client: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");

            return new ClientResult { Succeeded = false, StatusCode = 500 };
        }

    }
     
   //DELETE missing
}
