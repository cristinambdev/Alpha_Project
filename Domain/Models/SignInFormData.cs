
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;


public class SignInFormData
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public bool IsPersistent { get; set; }
}
