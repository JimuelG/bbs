using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Address = user.Address,
            Contact = user.Contact,
            Role = roles.FirstOrDefault()!
        });
    }

    // REGISTER NEW USER
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            MiddleName = registerDto.MiddleName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Address = registerDto.Address,
            Contact = registerDto.Contact,
            IdUrl = registerDto.IdUrl,
            IsIdVerified = false
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

        await userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Registration successful" });
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
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null) return NotFound("User not found");

        var roles = await userManager.GetRolesAsync(user);

        return Ok (new UserDto
        {   
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName!,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Contact = user.Contact,
            Address = user.Address,
            IdUrl = user.IdUrl,
            IsIdVerified = user.IsIdVerified,
            Role = roles.FirstOrDefault() ?? "Resident"
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Verify(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null) return NotFound();

        user.IsIdVerified = true;

        await unit.Complete();

        return Ok( new { message = "Updated succefully" });
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
