using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? Image { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName ="date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [Precision(18, 2)]
    public decimal? Budget { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; } 
    public StatusEntity Status { get; set; } = null!;

    public ICollection<ProjectUserEntity> ProjectUsers { get; set; } = [];
    public ICollection<ProjectClientEntity> ProjectClients { get; set; } = [];


}
