using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class AppContextSeed
{

    public static async Task SeedAsync(AppDbContext context,
        UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Staff"))
        {
            await roleManager.CreateAsync(new IdentityRole("Staff"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        
        
    }
}
