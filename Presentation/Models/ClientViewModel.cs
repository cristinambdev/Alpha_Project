using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Models;

public class ClientViewModel
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;

    public string? Email { get; set; }

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }
    public string? Phone { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public Client Client { get; set; } = null!;

    //public IEnumerable<StatusEntity> StatusList { get; set; } = new List<StatusEntity>();

    public List<SelectListItem> StatusList { get; set; } = new(); // Chat GPT to match the assigned data


}
