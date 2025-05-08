namespace Presentation.Models;

public class UserViewModel
{
    public string Id { get; set; } = null!;
    public string? UserImage { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? JobTitle { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } =string.Empty; // by chat gpt to show roles in view
}
