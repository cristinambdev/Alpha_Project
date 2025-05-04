using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Models;

public class MiniProjectsViewModel
{
    public IEnumerable<MiniProjectViewModel> MiniProjects { get; set; } = [];

    public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();  


    public AddMiniProjectViewModel AddMiniProjectForm { get; set; } = new();

    public EditMiniProjectViewModel EditMiniProjectForm { get; set; } = new();

}
