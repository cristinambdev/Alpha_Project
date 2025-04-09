using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditClientViewModel
{
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

    [Display(Name = "Address", Prompt = "Enter address")]
    [DataType(DataType.Text)]
    public string? Address { get; set; }

    [Display(Name = "Phone", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Display(Name = "Date", Prompt = "Enter a date")]
    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    [Display(Name = "Status", Prompt = "Enter a status")]
    [DataType(DataType.Text)]
    public string? Status { get; set; }

}
