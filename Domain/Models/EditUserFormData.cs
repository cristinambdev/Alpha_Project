﻿namespace Domain.Models;

public class EditUserFormData
{
    public string Id { get; set; } = null!;
    public string? UserImage { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? JobTitle { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? StreetName { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
}
