namespace Presentation.Models;

public class UsersViewModel
{
    public IEnumerable<UserViewModel> Users { get; set; } = [];

    public AddMemberViewModel AddMemberForm { get; set; } = new();

    public EditMemberViewModel EditMemberForm { get; set; } = new();
}
