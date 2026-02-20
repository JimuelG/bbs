using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager, IUnitOfWork unit) : BaseApiController
{
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();

        var user = await signInManager.UserManager.GetUserByEmail(User);
        var roles = await userManager.GetRolesAsync(user);

        return Ok(new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.Resident?.FirstName ?? "N/A",
            LastName = user.Resident?.LastName ?? "N/A",
            PhoneNumber = user.PhoneNumber ?? "N/A",
            Purok = user.Resident?.Purok ?? "N/A",
            IdUrl = user.IdUrl ?? "N/A",
            IsIdVerified = user.IsIdVerified,
            Role = roles.FirstOrDefault() ?? "Resident"
        });
    }

    // REGISTER NEW USER
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (await userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            return BadRequest("Email is already taken.");
        }

        var residentProfile = new Resident
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Purok = registerDto.Purok,
            IsHeadOfFamily = false,
            MonthlyIncome = 0
        };

        var user = new AppUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            IdUrl = registerDto.IdUrl,
            IsIdVerified = false,
            Resident = residentProfile
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "Resident");

        return Ok(new { message = "Registration successful. Please wait for ID verification." });
    }

    // [Authorize(Roles = "Admin, Staff")]
    [HttpGet("residents")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetResidents()
    {
        var users = await userManager.Users.Include(u => u.Resident)
            .ToListAsync();

        var userInfos = new List<UserInfoDto>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            userInfos.Add(new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.Resident?.FirstName ?? "N/A",
                LastName = user.Resident?.LastName ?? "N/A",
                PhoneNumber = user.PhoneNumber ?? "N/A",
                Purok = user.Resident?.Purok ?? "N/A",
                IdUrl = user.IdUrl ?? "N/A",
                IsIdVerified = user.IsIdVerified,
                Role = roles.FirstOrDefault() ?? "Resident"
            });
        }

        return Ok(userInfos);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }

    [HttpGet("auth-status")]
    public ActionResult GetAuthState()
    {
        return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
    }

    // [Authorize(Roles = "Admin, Staff")]
    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserInfoDto>> GetUserById(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null) return NotFound("User not found");

        var roles = await userManager.GetRolesAsync(user);

        return Ok (new UserInfoDto
        {   
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.Resident?.FirstName ?? "N/A",
            LastName = user.Resident?.LastName ?? "N/A",
            PhoneNumber = user.PhoneNumber ?? "N/A",
            Purok = user.Resident?.Purok ?? "N/A",
            IsIdVerified = user.IsIdVerified,
            Role = roles.FirstOrDefault() ?? "Resident"
        });
    }

    [HttpPut("verify/{id}")]
    public async Task<ActionResult> Verify(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null) return NotFound();

        user.IsIdVerified = true;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded) return BadRequest("Failed to update user status");

        return Ok( new { message = "Updated successfully" });
    }

    [HttpPatch("change-password/{userId}")]
    public async Task<ActionResult> ChangePassword(string userId, ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null) return NotFound("User not found");

        var isOldPassword = await userManager.CheckPasswordAsync(user, dto.OldPassword);

        if (!isOldPassword) return BadRequest("Incorrect old password");

        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            var passwordChangeResult = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!passwordChangeResult.Succeeded) return BadRequest(passwordChangeResult);
        }

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);

        return Ok( new { message = "Password change successfully" });
    }
}
