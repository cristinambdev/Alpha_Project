using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Data.Entities;

public class MiniProjectEntity
{

    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectImage { get; set; } = null!;
    
    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime? StartDate { get; set; } = DateTime.Now;

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; } = DateTime.Now.Date; 

    public string? ClientName { get; set; }
    public int? Budget { get; set; } 
    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;



}
