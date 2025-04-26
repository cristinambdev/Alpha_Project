namespace Presentation.Models;

public class ClientsViewModel
{
    public IEnumerable<ClientViewModel> Clients { get; set; } = [];

    public AddClientViewModel AddClientForm { get; set; } = new();

    public EditClientViewModel EditClientForm { get; set; } = new();
}
