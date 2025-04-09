using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditMemberViewModel
{
    public int Id { get; set; }

    [Display(Name = "Member Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? MemberImage { get; set; }

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
    public string JobTitle { get; set; } = null!;

    [Display(Name = "Address", Prompt = "Your address")]
    [DataType(DataType.Text)]
    public string? Address { get; set; }

    [Display(Prompt = "Day")]
    [DataType(DataType.Text)]
    public string? Day { get; set; }

    [Display(Prompt = "Month")]
    [DataType(DataType.Text)]
    public string? Month { get; set; }

    [Display(Prompt = "Year")]
    [DataType(DataType.Text)]
    public string? Year { get; set; }

}

