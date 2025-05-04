using Domain.Models;

namespace Presentation.Models;

public class ProjectsViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];

    public AddProjectViewModel AddProjectForm { get; set; } = new();

    public EditProjectViewModel EditProjectForm { get; set; } = new();

    public IEnumerable<UserViewModel> Users { get; set; } = [];
    public IEnumerable<ClientViewModel> Clients { get; set; } = [];
}
