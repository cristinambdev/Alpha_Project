using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class EditMiniProjectFormData
{
    public string? Id { get; set; }
    public IFormFile? ProjectImage { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string ClientName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public string Member { get; set; } = null!;
    public int? Budget { get; set; }
    public int StatusId { get; set; }

    // by chatgpt to show the current projectimage when populating the edit form:
    public string? ExistingImageUrl { get; set; }
}


