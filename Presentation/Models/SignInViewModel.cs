using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignInViewModel
{
    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter Email Address")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z)(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember me")]
    public bool IsPersistent { get; set; }
}
