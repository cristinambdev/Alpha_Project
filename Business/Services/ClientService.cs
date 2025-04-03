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
    Task<ClientResult> CreateClientsAsync(AddClientFormData formData);
    Task<ClientResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>(); // dynamic mapping

    }

    public async Task<ClientResult> CreateClientsAsync(AddClientFormData formData)
    {
        if (formData == null)
            return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Form data can't be null" };

        var existsResult = await _clientRepository.ExistsAsync(x => x.ClientName == formData.ClientName);
        if (existsResult.Succeeded)
            return new ClientResult { Succeeded = false, StatusCode = 409, Error = "Client with same name already exists." };



        try
        {
            var clientEntity = formData.MapTo<ClientEntity>();

            var result = await _clientRepository.AddAsync(clientEntity);

            //var clientEntity = new ClientEntity // suggested by Chat Gpt as the dynamic mapping didn't wok 
            //{
            //    ClientName = formData.ClientName,
            //    Email = formData.Email,
            //    Phone = formData.Phone,
            //    Address = formData.Address,
            //    Date = formData.Date,
            //    Status = formData.Status,

            //};
            //var result = await _clientRepository.AddAsync(clientEntity);


            return new ClientResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ClientResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    //UPDATE & DELETE missing
}
