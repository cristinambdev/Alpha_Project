namespace Domain.Models;

public class MiniProject
{
    public string Id { get; set; } = null!;

    public string? ProjectImage { get; set; }
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ClientName { get; set; }
    public DateTime? StartDate { get; set; } 

    public DateTime? EndDate { get; set; } 
    public int? Budget { get; set; }

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;
}