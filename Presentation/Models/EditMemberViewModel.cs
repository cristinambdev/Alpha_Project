using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditMemberViewModel
{
    public string? Id { get; set; }

    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? UserImage { get; set; }

    [Display(Name = "First Name", Prompt = "Your first name")]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Your last name")]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Your email address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone", Prompt = "Your phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Display(Name = "Job Title", Prompt = "Your job title")]
    [DataType(DataType.Text)]
    public string? JobTitle { get; set; } = null!;

    [Display(Name = "Address", Prompt = "Enter Address")]
    [DataType(DataType.Text)]
    public string? StreetName { get; set; }

    [Display(Name = "Postal Code", Prompt = "PostalCode")]
    [DataType(DataType.Text)]
    public string? PostalCode { get; set; }

    [Display(Name = "City", Prompt = "City")]
    [DataType(DataType.Text)]
    public string? City { get; set; }

    [Display(Name = "Role", Prompt = "Role")]
    [DataType(DataType.Text)]
    public string? Role { get; set; }
}

