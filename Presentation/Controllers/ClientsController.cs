using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Presentation.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Presentation.Controllers;

public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> Index()
    {
        var clientsResult = await _clientService.GetClientsAsync();
        return View(clientsResult);
    }


    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientViewModel form)
    {
        var addClientFormData = form.MapTo<AddClientFormData>();

        var result = await _clientService.CreateClientsAsync(addClientFormData);

        return Json(new { });

    }


    [HttpPost]
    public IActionResult EditClient(EditClientViewModel form)
    {
        if (!ModelState.IsValid)
        //manage information through API
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary( // kvp - key value pair , JSON object
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                 );
            return BadRequest(new { success = false, errors });
        }
        // Send Data to clientService

        return Ok(new { success = true });

        ////with "ClientService"
        //var result = await _clientService.UpdateClientAsync(form);
        //if (result)
        //{
        //    return Ok(new { success = true });
        //}
        //else
        //{
        //    return Problem("Unable to edit data");
        //}
    }
}
