using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string? UserImage { get; set; }

    [ProtectedPersonalData]
    public string? FirstName { get; set; }

    [ProtectedPersonalData]
    public string? LastName { get; set; } 

    public string? JobTitle { get; set; }

    public string? Role { get; set; } 
    public virtual UserAddressEntity? Address { get; set; }


    public ICollection<ProjectUserEntity> ProjectUsers { get; set; } = [];


}
