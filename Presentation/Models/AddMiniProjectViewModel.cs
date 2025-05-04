using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AddMiniProjectViewModel
{
  


    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }

    [Display(Name ="Project Name", Prompt = "Project Name")]
    [Required(ErrorMessage ="Required")]
    [DataType(DataType.Text)]
    public string Title { get; set; } = null!;

    [Display(Name = "Client Name", Prompt = "Client Name")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Description", Prompt = "Type something")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; } 

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

   
    [Display(Name = "Budget", Prompt = "")]
    public int? Budget {  get; set; }

    public int StatusId { get; set; }


}
