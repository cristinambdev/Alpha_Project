using Business.Models;
using Business.Services;
using Data.Entities;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;


namespace Presentation.Controllers;

public class ClientsController(IClientService clientService, IWebHostEnvironment env) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IWebHostEnvironment _env = env;

    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> Index()
    {
        var clientResult = await _clientService.GetClientsAsync();

        if (!clientResult.Succeeded)
        {
            // handle error (optional)
        }

        var clients = clientResult.Result?.Select(c => new ClientViewModel
        {
            Client = c
        }) ?? new List<ClientViewModel>();

        var viewModel = new ClientsViewModel
        {
            Clients = clients
        };

        return View(viewModel);
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


       //upload image handling
        if (form.ClientImage != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);

            // By Chat GPT: restructuring of code for image filename and storing path
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);
            //var filePath = Path.Combine(uploadFolder, $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.ClientImage.CopyToAsync(stream);
            }

            // By Chat GPT: Set image path to display later
            client.Image = Path.Combine("uploads", fileName).Replace("\\", "/");
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
    public async Task<IActionResult> EditClient(EditClientViewModel form)
    {
        try
        {
            // Log the incoming form data
            Debug.WriteLine($"EditClient called with ID: {form.Id}");

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
            var editClientFormData = form.MapTo<EditClientFormData>();
            Debug.WriteLine($"Mapped form data, ClientName: {editClientFormData.ClientName}");

            //image handling
            if (form.ClientImage != null)
            {
                Debug.WriteLine("Processing image...");
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.ClientImage.CopyToAsync(stream);
                }

                // By Chat GPT: Save the path
                editClientFormData.Image = filePath; // Save the path
            }
            Debug.WriteLine("Calling UpdateClientAsync...");
            var result = await _clientService.UpdateClientAsync(editClientFormData);
            //var result = await _clientService.UpdateClientAsync(form);
            Debug.WriteLine($"Service result: Success={result.Succeeded}, StatusCode={result.StatusCode}, Error={result.Error}");

            if (result.Succeeded)
            {
                return Ok(new { success = true });
            }
            else
            {
                return Problem("Unable to edit data");
            }

        }
        catch (Exception ex)
        {
            // Log the full exception details
            Debug.WriteLine($"Exception in EditClient: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return Problem($"Exception: {ex.Message}");
        }
    }


}
