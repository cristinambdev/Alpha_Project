using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ClientImage { get; set; } = null!;

    public string ProjectName { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string TimeLeft { get; set; } = null!;

    public IEnumerable<string> Users { get; set; } = [];

    public IEnumerable<string> Clients { get; set; } = [];
}
