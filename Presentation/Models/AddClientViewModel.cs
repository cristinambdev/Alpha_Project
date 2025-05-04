using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AddClientViewModel
{
    [Display(Name = "Client Image", Prompt = "Enter image")]
    [DataType(DataType.Upload)]
    public IFormFile? ClientImage { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter client name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage= "Required")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
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
    public int? Status { get; set; }
}
