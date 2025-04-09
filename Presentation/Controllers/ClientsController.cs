using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;


namespace Presentation.Controllers;

public class ClientsController(IClientService clientService, IWebHostEnvironment env) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IWebHostEnvironment _env = env;

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
        //map formData 
        var client = form.MapTo<AddClientFormData>();
        //var result = await _clientService.CreateClientAsync(form);

        //upload image handling
        if (form.ClientImage != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.ClientImage.CopyToAsync(stream);
            }
        }
            
       var result = await _clientService.CreateClientAsync(client);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }
        else
        {
            return Problem("Unable to submit data");
        }

      

    }


    [HttpPost]
    public async Task<IActionResult> EditClient(EditClientFormData form)
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
        //var editClientFormData = form.MapTo<EditClientFormData>();
        //var result = await _clientService.UpdateClientAsync(editClientFormData);
        var result = await _clientService.UpdateClientAsync(form);
        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }
        else
        {
            return Problem("Unable to edit data");
        }

    }


}
