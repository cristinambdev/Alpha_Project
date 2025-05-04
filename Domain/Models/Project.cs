namespace Domain.Models;

public class Project
{
    public int Id { get; set; } 

    public string? ClientImage { get; set; }
    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; } 
    public decimal? Budget { get; set; }
    public Client Client { get; set; } = null!;

    public User User { get; set; } = null!;

    public Status Status { get; set; } = null!;
}