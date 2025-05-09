using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditMiniProjectViewModel
{


    public string Id { get; set; } = null!;

    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }


    [Display(Name = "Project Name", Prompt = "Project Name")]
    [DataType(DataType.Text)]
    public string Title { get; set; } = null!;

    [Display(Name = "Description", Prompt = "Type something")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Client Name")]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Budget")]
    [DataType(DataType.Currency)]
    public int? Budget { get; set; }

    public int StatusId { get; set; }
    public IEnumerable<SelectListItem>? StatusOptions { get; set; }

}
