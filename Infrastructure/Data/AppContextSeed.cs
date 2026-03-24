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
        
        if (!userManager.Users.Any(x => x.UserName == "dayriesmariano5@gmail.com"))
        {
            var adminProfile = new Resident
            {
                FirstName = "Dayries",
                LastName = "Mariano",
                Purok = "Purok 2",
                IsHeadOfFamily = false,
                MonthlyIncome = 0
            };
            
            var user = new AppUser
            {
                UserName = "dayriesmariano5@gmail.com",
                Email = "dayriesmariano5@gmail.com",
                IsIdVerified = true,
                PhoneNumber = "09123456789",
                Resident = adminProfile
            };

            var result = await userManager.CreateAsync(user, "Password1!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Staff");
            }
        }
    }
}
