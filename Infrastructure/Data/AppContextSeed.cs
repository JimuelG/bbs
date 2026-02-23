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

        if (!context.BarangayOfficials.Any())
        {
            var officials = new List<BarangayOfficial>
            {
                new BarangayOfficial
                {
                    FirstName = "Carlito",
                    LastName = "Mariano",
                    MiddleName = "R.",
                    Position = "Barangay Captain",
                    Rank = 1
                },
                new BarangayOfficial
                {
                    FirstName = "Abegail", 
                    LastName = "Valdoz", 
                    MiddleName = "N.", 
                    Position = "Barangay Secretary", 
                    Rank = 2
                }
            };

            context.BarangayOfficials.AddRange(officials);
            await context.SaveChangesAsync();
        }
        
        if (!userManager.Users.Any(x => x.UserName == "jimuelgaas@gmail.com"))
        {
            var adminProfile = new Resident
            {
                FirstName = "Jimuel",
                LastName = "Gaas",
                Purok = "Zone 1",
                IsHeadOfFamily = false,
                MonthlyIncome = 0
            };
            
            var user = new AppUser
            {
                UserName = "jimuelgaas@gmail.com",
                Email = "jimuelgaas@gmail",
                IsIdVerified = true,
                PhoneNumber = "09386089484",
                Resident = adminProfile
            };

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Staff");
            }
        }
    }
}
