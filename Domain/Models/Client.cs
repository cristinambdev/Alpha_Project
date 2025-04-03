namespace Domain.Models;

public class Client
{
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? Status { get; set; }
}
