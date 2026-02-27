using API.DTOs;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager, IUnitOfWork unit, IMapper mapper) : BaseApiController
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

        unit.Repository<Resident>().Add(residentProfile);

        var saved = await unit.Complete();
        if (!saved) return BadRequest("Problem creating resident profile.");

        var user = new AppUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            IdUrl = registerDto.IdUrl,
            IsIdVerified = false,
            ResidentId = residentProfile.Id,
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

    [HttpGet("residents/verified")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetVerifiedResidents()
    {
        var spec = new VerifiedResidentsWithUserSpecification();

        var residents = await unit.Repository<Resident>().ListAsync(spec);

        return Ok(mapper.Map<IEnumerable<UserInfoDto>>(residents));
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

    [HttpPost("upload-id-card")]
    [Authorize]
    public async Task<ActionResult> UploadIdCard([FromForm] IFormFile file)
    {
        
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound("User not found");

        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension)) return BadRequest("Invalid file type. Only JPG and PNG are allowed.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "ids");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fileName = $"{user.Id}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = $"/images/ids/{fileName}";

        user.IdUrl = relativePath;
        user.IsIdVerified = false;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded) return BadRequest("Failed to upload ID card");

        return Ok(new { url = relativePath });
    }
}
