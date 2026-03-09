using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class StaffController(IUnitOfWork unit, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateStaff(CreateStaffDto dto)
    {
        var staff = new Staff
        {
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Position = dto.Position,
            BirthDate = dto.BirthDate,
            Email = dto.Email
        };

        unit.Repository<Staff>().Add(staff);

        var newStaff = await unit.Complete();

        return Ok( new { message = "Staff created successfully" } );
    }

    [HttpGet()]
    public async Task<ActionResult<IReadOnlyList<StaffDto>>> GetStaffs()
    {
        var staffs = await unit.Repository<Staff>().ListAllAsync();

        return Ok(mapper.Map<IReadOnlyList<StaffDto>>(staffs));
    }
}
