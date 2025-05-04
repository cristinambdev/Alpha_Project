using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

[Index(nameof(ClientName), IsUnique = true)]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ClientName { get; set; } = null!;

    public string? Email { get; set; }

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }
    public string? Phone { get; set; }

    public DateTime? Date { get; set; } = DateTime.Now.Date;

    public string? Status { get; set; }

    public string? Image { get; set; }

    public ICollection<ProjectClientEntity> ProjectClients { get; set; } = new List<ProjectClientEntity>();


}
