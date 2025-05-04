using Domain.Models;

namespace Presentation.Models;

public class MiniProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ProjectImage { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? Budget {  get; set; }

    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;

    public MiniProject MiniProject { get; set; } = null!;

}
