using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key]
    public string UserId { get; set; } = null!;

    public string? StreetName { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public virtual UserEntity User { get; set; } = null!;

}