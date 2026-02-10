using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public string FirstName{ get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string IdUrl { get; set; } = string.Empty;
    public bool IsIdVerified { get; set; } = false;

}
