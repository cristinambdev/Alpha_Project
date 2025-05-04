using System.ComponentModel;

namespace Domain.Models;

public class AddProjectFormData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public string Members { get; set; } = null!;
    public decimal? Budget { get; set; }

    public string UserId { get; set; } = null!;

    public int StatusId { get; set; }
}
