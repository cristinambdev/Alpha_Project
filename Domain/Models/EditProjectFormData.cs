using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class EditProjectFormData
{
    public string? Id { get; set; }
    public IFormFile? ClientImage { get; set; }
    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ClientId { get; set; } = null!;

    public int UserId { get; set; } 

    public int StatusId { get; set; }
}
