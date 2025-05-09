using Business.Services;
using Data.Contexts;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService, AppDbContext context, IStatusService statusService, IImageService imageService) : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly IStatusService _statusService = statusService;
    private readonly AppDbContext _context = context;
    private readonly IImageService _imageService = imageService;


    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> Index()
    {
        var clientResult = await _clientService.GetClientsAsync();
        var statusResult = await _statusService.GetStatusesAsync();

        if (!clientResult.Succeeded)
        {
            // handle error (optional)
        }

        // Suggested by Chat GPT. Map the status result into the view and form select
         var statusList = statusResult.Result?
        .Select(status => new SelectListItem
        {
            Value = status.StatusName ?? "", 
            Text = status.StatusName ?? " "
        })
        .ToList() ?? new List<SelectListItem>();
        // Add Active and Inactive only if they are not present in the result
        if (!statusList.Any(x => x.Value == "Active"))
            statusList.Add(new SelectListItem { Value = "Active", Text = "Active" });

        if (!statusList.Any(x => x.Value == "Inactive"))
            statusList.Add(new SelectListItem { Value = "Inactive", Text = "Inactive" });

        var clients = clientResult.Result?.Select(client => new ClientViewModel
        {
            Id = client.Id,
            Client = client,
            StatusList = statusList


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
        // Manually map the AddClientViewModel to AddClientFormData
        var client = form.MapTo<AddClientFormData>();
        //var client = new AddClientFormData
        //{
        //    ClientName = form.ClientName,
        //    Email = form.Email,
        //    StreetName = form.StreetName,
        //    PostalCode = form.PostalCode,
        //    City = form.City,
        //    Phone = form.Phone,
        //    Date = form.Date,
        //    Status = form.Status.ToString(),
        //};


        //upload image handling
        string? imagePath = null;
        if (form.ClientImage != null)
        {
            imagePath = await _imageService.SaveImageAsync(form.ClientImage, "clients");

        }
        //string? imagePath = null;
        //if (form.ClientImage != null)
        //{
        //    var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "clients");
        //    Directory.CreateDirectory(uploadFolder);

        //    // By Chat GPT: restructuring of code for image filename and storing path
        //    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.ClientImage.FileName)}";
        //    var filePath = Path.Combine(uploadFolder, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await form.ClientImage.CopyToAsync(stream);
        //    }

        //    imagePath = $"/uploads/clients/{fileName}";
        //}
        client.Image = imagePath;

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
    public async Task<IActionResult> EditClient(EditClientViewModel model)
    {
        

        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return BadRequest(new { errors });
        }

        string? imagePath = null;
        if (model.ClientImage != null)
        {
            imagePath = await _imageService.SaveImageAsync(model.ClientImage, "clients");

        }

        //string? imagePath = null;
        //if (model.ClientImage != null)
        //{
        //    // Handle image upload
        //    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ClientImage.FileName)}";
        //    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "clients");
        //    Directory.CreateDirectory(uploadPath);
        //    var filePath = Path.Combine(uploadPath, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await model.ClientImage.CopyToAsync(stream);
        //    }

        //    imagePath = $"/uploads/clients/{fileName}";
        //}

        var entity = await _context.Clients.FindAsync(model.Id);
        if (entity == null)
            return NotFound();
        //entity = model.MapTo<ClientEntity>();
        entity.ClientName = model.ClientName;
        entity.Email = model.Email;
        entity.Phone = model.Phone;
        entity.StreetName = model.StreetName;
        entity.PostalCode = model.PostalCode;
        entity.City = model.City;
        entity.Date = model.Date;
        entity.Status = model.Status;

        if (imagePath != null)
        {
            entity.Image = imagePath;
        }
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetClientData(int clientId)
    {

        var client = await _context.Clients
             .Where(c => c.Id == clientId.ToString()) // help from Chat GPT to poupulate items
             .Select(c => new

             {
                 c.Id,
                 c.ClientName,
                 c.Email,
                 c.Phone,
                 c.StreetName,
                 c.PostalCode,
                 c.City,
                 c.Date,
                 c.Status,
                 ClientImage = c.Image

             })
            .FirstOrDefaultAsync();


        if (client == null)
        {
            return NotFound();
        }

        return Json(client);


    }


    [HttpPost]
    public async Task<IActionResult> DeleteClient(string id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
            return NotFound();
        var deleteResult = await _clientService.DeleteClientAsync(id);

        if (deleteResult.Succeeded)
        {
            return Json(new { success = true, message = "Client deleted successfully!" }); // Return JSON for AJAX
        }
        else
        {
            return Json(new { success = false, message = "Failed to delete client.", errors = deleteResult.Error }); // Return JSON with errors
        }
    }

    [HttpGet]
    public async Task<JsonResult> SearchClients(string term)
    {
        if (string.IsNullOrEmpty(term))
            return Json(new List<object>());

        var clients = await _context.Clients
            .Where(x => x.ClientName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new 
            { x.Id,
                ClientImage = x.Image ?? "",
                x.ClientName })
            .ToListAsync();

        return Json(clients);
    }

}

