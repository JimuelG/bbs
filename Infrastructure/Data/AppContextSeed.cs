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
        if (!await roleManager.RoleExistsAsync("Resident"))
        {
            await roleManager.CreateAsync(new IdentityRole("Resident"));
        }
        
        if (!userManager.Users.Any(x => x.UserName == "jimuelgaas@gmail.com"))
        {
            var user = new AppUser
            {
                UserName = "jimuelgaas@gmail.com",
                Email = "jimuelgaas@gmail",
                FirstName = "Jimuel",
                LastName = "Gaas",
                IsIdVerified = true,
                Contact = "09386089484"
            };

            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Staff");
        }
    }
}
