namespace Domain.Models;

public class EditClientFormData
{
    public string? Id { get; set; }
    public string? ClientImage { get; set; }
    public string ClientName { get; set; } = null!;

    public string? Email { get; set; }

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }
    public string? Phone { get; set; }

    public DateTime? Date { get; set; }

    public int? StatusId { get; set; }
}

