using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class EditMiniProjectFormData
{
    public string? Id { get; set; }
    public string? ProjectImage { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string ClientName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public int? Budget { get; set; }
    public int StatusId { get; set; }
}


