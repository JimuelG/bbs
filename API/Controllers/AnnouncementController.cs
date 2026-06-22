using API.DTOs;
using API.Hubs;
using API.RequestHelpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AnnouncementController(IUnitOfWork unit,
    IMapper mapper,
    ITtsService ttsService,
    ISmsService smsService,
    UserManager<AppUser> userManager,
    IHubContext<DeviceHub> hubContext) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AnnouncementDto>>> GetAnnouncements([FromQuery] AnnouncementSpecParams specParams)
    {
        var spec = new AnnouncementSpecification(specParams);
        var countSpec = new AnnouncementSpecification(specParams);

        var totalItems = await unit.Repository<Announcement>().CountAsync(countSpec);
        var announcements = await unit.Repository<Announcement>().ListAsync(spec);

        var data = mapper.Map<IReadOnlyList<Announcement>, IReadOnlyList<AnnouncementDto>>(announcements);

        return Ok(new Pagination<AnnouncementDto>(
            specParams.PageIndex,
            specParams.PageSize,
            totalItems,
            data
        ));
    }

    [HttpGet("all")]
    public async Task<ActionResult<Announcement>> GetAllAnnouncements()
    {
        var announcements = await unit.Repository<Announcement>().ListAllAsync();

        var data = mapper.Map<IReadOnlyList<Announcement>, IReadOnlyList<AnnouncementDto>>(announcements);

        return Ok(data);

    }

    [HttpGet("latest")]
    public async Task<ActionResult<Announcement>> GetLatest()
    {
        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        var spec = new LatestAnnouncementSpecification(phTime);
        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec);
            
        if (announcement == null) return NoContent();

        announcement.IsPlayed = true;
        unit.Repository<Announcement>().Update(announcement);
        await unit.Complete();
        
        var dto = new AnnouncementResponseDto
        {
            Id = announcement.Id,
            Message = announcement.Message,
            AudioUrl = announcement.AudioUrl,
            IsEmergency = announcement.IsEmergency,
            ScheduledAt = announcement.ScheduledAt
        };

        return Ok(dto);
    }

    [HttpPost("trigger/{id:int}")]
    public async Task<IActionResult> TriggerManual(int id)
    {
        const string deviceId = "barangay-rpi-001";

        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound("Announcement not found.");

        if(string.IsNullOrWhiteSpace(announcement.AudioUrl))
            return BadRequest("This announcement has no generated audio.");

        var spec = new AnnouncementManualTriggerSpecification();
        var activeManualTriggers = await unit.Repository<Announcement>().ListAsync(spec);

        foreach (var item in activeManualTriggers)
        {
            if (item.Id == announcement.Id) continue;

            item.ManualTriggerActive = false;
            unit.Repository<Announcement>().Update(item);
        }

        announcement.ManualTriggerActive = true;

        unit.Repository<Announcement>().Update(announcement);

        if (!await unit.Complete())
            return BadRequest("Problem activating manual trigger.");

        var audioUrl = announcement.AudioUrl;

        if (audioUrl.StartsWith("/audio/"))
        {
            audioUrl = audioUrl.Replace("/audio/", "/api/audio/");
        }

        await hubContext.Clients.Group(deviceId).SendAsync("PlayAudio", new
        {
           audioUrl,
           announcementId = announcement.Id 
        });
        
        return Ok(new
        {
           message = "Play command sent to RPi.",
           deviceId,
           announcementId = announcement.Id,
           audioUrl = announcement.AudioUrl 
        });
    }

    [HttpPost("stop-rpi")]
    public async Task<IActionResult> StopRpiAudio()
    {
        const string deviceId = "barangay-rpi-001";

        await hubContext.Clients.Group(deviceId).SendAsync("StopAudio");

        var announcements = await unit.Repository<Announcement>().ListAllAsync();

        foreach (var announcement in announcements.Where(x => x.ManualTriggerActive))
        {
            announcement.ManualTriggerActive = false;
            unit.Repository<Announcement>().Update(announcement);
        }

        await unit.Complete();

        return Ok(new
        {
           message = "Stop command sent to RPi.",
           deviceId 
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> GetAnnounceById(int id)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);

        if (announcement == null) return NotFound("No announcement found");

        var mapped = mapper.Map<AnnouncementDto>(announcement);

        return Ok(mapped);
    }

    [HttpGet("display")]
    public async Task<ActionResult<DisplayAnnouncementDto>> GetDisplay()
    {
        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
        var spec = new LatestAnnouncementSpecification(phTime);
        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec); 
        if (announcement == null) return NoContent();

        var dto = new DisplayAnnouncementDto
        {
            Title = announcement.Title,
            Message = announcement.Message,
            IsEmergency = announcement.IsEmergency

        };

        return Ok(dto);
    }

    // [Authorize(Roles = "Staff")]
    [HttpPost]
    public async Task<ActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
    {
        var audioUrl = await ttsService.GenerateSpeechAsync(dto.Message, dto.IsEmergency, dto.LanguageCode);

        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        var announcement = mapper.Map<Announcement>(dto);
        announcement.CreateAt = phTime;
        announcement.AudioUrl = audioUrl;

        unit.Repository<Announcement>().Add(announcement);
        await unit.Complete();

        return CreatedAtAction(nameof(GetAnnounceById), 
            new { id = announcement.Id}, 
            mapper.Map<AnnouncementDto>(announcement));
    }

    [HttpPost("preview")]
    public async Task<ActionResult> PreviewAnnouncement([FromBody] PreviewDto dto)
    {
        var audioUrl = await ttsService.GenerateSpeechAsync(dto.Message, dto.IsEmergency, dto.LanguageCode);

        return Ok(new { AudioUrl = audioUrl });
    }

    [HttpPut("{id}/played")]
    public async Task<ActionResult> MarkAsPlayed(int id)
    {

        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound();

        announcement.IsPlayed = true;
        unit.Repository<Announcement>().Update(announcement);
        await unit.Complete();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAnnouncement(int id)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound();

        unit.Repository<Announcement>().Remove(announcement);
        await unit.Complete();

        return NoContent();
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> Ping()
    {
        var statusList = await unit.Repository<RPiStatus>().ListAllAsync();
        var status = statusList.FirstOrDefault();

        if (status == null)
        {
            status = new RPiStatus { LastSeen = DateTime.UtcNow };
            unit.Repository<RPiStatus>().Add(status);
        }
        else
        {
            status.LastSeen = DateTime.UtcNow;
            unit.Repository<RPiStatus>().Update(status);
        }

        await unit.Complete();
        return Ok();
    }

    [HttpGet("rpi-status")]
    public async Task<ActionResult<object>> GetRPiStatus()
    {
        var statuses = await unit.Repository<RPiStatus>().ListAllAsync();
        var rpi = statuses.FirstOrDefault();

        if (rpi == null) return Ok(new { isOnline = false, lastSeen = (DateTime?)null });

        var diff = DateTime.UtcNow - rpi.LastSeen;
        var isOnline = Math.Abs(diff.TotalSeconds) < 90;

        var now = DateTime.UtcNow;
        var last = rpi.LastSeen;
        var secondsGap = (now - last).TotalSeconds;

        // Check your terminal/console to see these values
        Console.WriteLine($"Current UTC: {now}, RPi LastSeen: {last}, Gap: {secondsGap}");

        return Ok(new
        {
            isOnline = isOnline,
            lastSeen = rpi.LastSeen
        });
    }

    [HttpPost("sms-broadcast")]
    public async Task<IActionResult> SmsBroadcastAnnouncement([FromBody] AnnouncementDto dto)
    {
        var residentNumber = await userManager.Users
            .Where(u => u.IsIdVerified && !string.IsNullOrWhiteSpace(u.PhoneNumber))
            .Select(u => u.PhoneNumber!)
            .ToListAsync();

        if (!residentNumber.Any())
        {
            return BadRequest(new { message = "No verified residents with valid contact numbers were found."});
        }

        var isSent = await smsService.SendAnnouncementAsync(residentNumber, dto.Message);

        if (!isSent)
        {
            return StatusCode(500, new { message = "An error occured while sending the SMS broadcast." });
        }

        return Ok(new { message = $"SMS announcement successfully broadcasted to {residentNumber.Count} residents." });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AnnouncementStatsDto>> GetStats()
    {
        var announcements = await unit.Repository<Announcement>().ListAllAsync();

        var phTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")
        );

        var stats = new AnnouncementStatsDto
        {
            BroadcastsSent = announcements.Count(x => x.IsPlayed),
            Scheduled = announcements.Count(x => x.ScheduledAt > phTime && !x.IsPlayed),
            Emergencies = announcements.Count(x => x.IsEmergency)
        };

        return Ok(stats);
    }

}