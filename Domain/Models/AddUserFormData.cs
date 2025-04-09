namespace Domain.Models;

public class AddUserFormData
{

    public string Id { get; set; } = null!;
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? JobTitle { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }
}
