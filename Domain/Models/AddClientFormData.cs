namespace Domain.Models;

public class AddClientFormData
{
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;

    public string? Email { get; set; }

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }
    public string? Phone { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }
}

