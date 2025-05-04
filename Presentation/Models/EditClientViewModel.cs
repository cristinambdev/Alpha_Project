using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditClientViewModel
{
    public List<SelectListItem> StatusList { get; set; } = new();
    public string? Id {get; set;}

    [Display(Name = "Client Image", Prompt = "Enter image")]
    [DataType(DataType.Upload)]
    public IFormFile? ClientImage { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Address", Prompt = "Enter Address")]
    [DataType(DataType.Text)]
    public string? StreetName { get; set; }


    [Display(Name = "Postal Code", Prompt = "Enter Postal Code")]
    [DataType(DataType.Text)]
    public string? PostalCode { get; set; }


    [Display(Name = "City", Prompt = "Enter City")]
    [DataType(DataType.Text)]
    public string? City { get; set; }

    [Display(Name = "Phone", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Display(Name = "Date", Prompt = "Enter a date")]
    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    [Display(Name = "Status", Prompt = "Enter a status")]
    [DataType(DataType.Text)]
    public string? Status { get; set; }

    // Chat GPT - Add this property to hold the available statuses for the dropdown
    
}
