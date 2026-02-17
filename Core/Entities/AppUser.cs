using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public string? IdUrl { get; set; }
    public bool IsIdVerified { get; set; }
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; }

}
