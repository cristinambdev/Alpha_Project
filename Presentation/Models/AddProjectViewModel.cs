using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AddProjectViewModel
{
    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }

    [Display(Name ="Project Name", Prompt = "Project Name")]
    [Required(ErrorMessage ="Required")]
    [DataType(DataType.Text)]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client Name", Prompt = "Client Name")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Description", Prompt = "Type something")]
    //[DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [Required(ErrorMessage = "Required")]
    //[DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    [Required(ErrorMessage = "Required")]
    //[DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Members", Prompt = "Add Project Members")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Text)]
    public string Members { get; set; } = null!;

    [Display(Name = "Budget", Prompt = "")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Currency)]
    public decimal Budget {  get; set; }

    public string UserId { get; set; } = null!;

    public int StatusId { get; set; }


}
