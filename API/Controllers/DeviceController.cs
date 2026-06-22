using API.DTOs;
using API.Hubs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class DeviceController(
    IHubContext<DeviceHub> hubContext, 
    IUnitOfWork unit,
    UserManager<AppUser> userManager,
    IConfiguration config
    ) : BaseApiController
{

    [HttpPost("{deviceId}/play")]
    public async Task<ActionResult> PlayAnnouncement(string deviceId, PlayAudioDto dto)
    {
        await hubContext.Clients.Group(deviceId).SendAsync("PlayAudio", new
        {
            audioUrl = dto.AudioUrl,
            announcementId = dto.AnnouncementId
        });

        return Ok(new { message = "Play command sent to device." });
    }

    [HttpPost("{deviceId}/stop")]
    public async Task<ActionResult> StopAudio(string deviceId)
    {
        await hubContext.Clients.Group(deviceId).SendAsync("StopAudio");

        return Ok(new { message = "Stop command sent to device." });
    }

    [HttpPost("{deviceId}/announcements/{announcementId}/play")]
    public async Task<ActionResult> PlayAnnouncementById(string deviceId, int announcementId)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(announcementId);

        if (announcement == null) return NotFound("Announcement not found.");

        if (string.IsNullOrWhiteSpace(announcement.AudioUrl)) return BadRequest("This announcement has no generated audio.");

        await hubContext.Clients.Group(deviceId).SendAsync("PlayAudio", new
        {
           audioUrl = announcement.AudioUrl,
           announcementId = announcement.Id 
        });

        return Ok(new
        {
           message = "Announcement play command sent to device.",
           deviceId,
           announcementId = announcement.Id,
           audioUrl = announcement.AudioUrl 
        });
    }

    [AllowAnonymous]
    [HttpGet("dashboard")]
    public async Task<ActionResult<RpiDashboardDto>> GetRpiDashboard(
        [FromHeader(Name = "X-Device-Key")] string? deviceKey)
    {
        var expectedKey = config["DeviceDashboard:Key"];

        if (!string.IsNullOrWhiteSpace(expectedKey) && deviceKey != expectedKey)
            return Unauthorized("Invalid device key.");

        var phTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")
        );

        var totalRegisteredResidents = await userManager.Users
            .Include(u => u.Resident)
            .CountAsync(u => u.Resident != null && !u.Resident.IsDeleted);

        var announcements = await unit.Repository<Announcement>().ListAllAsync();

        var activeAnnouncements = announcements
            .Where(x => x.IsActive)
            .ToList();

        var latestAnnouncements = activeAnnouncements
            .OrderByDescending(x => x.ScheduledAt)
            .Take(8)
            .Select(x => new RpiAnnouncementDto
            {
                Id = x.Id,
                Title = x.Title,
                Message = x.Message,
                ScheduledAt = x.ScheduledAt,
                IsPlayed = x.IsPlayed,
                IsEmergency = x.IsEmergency
            })
            .ToList();

        var dto = new RpiDashboardDto
        {
            TotalRegisteredResidents = totalRegisteredResidents,
            TotalAnnouncements = activeAnnouncements.Count,
            PlayedAnnouncements = activeAnnouncements.Count(x => x.IsPlayed),
            PendingAnnouncements = activeAnnouncements.Count(x => !x.IsPlayed),
            ScheduledAnnouncements = activeAnnouncements.Count(x =>
                !x.IsPlayed &&
                x.ScheduledAt > phTime
            ),
            Emergencies = activeAnnouncements.Count(x => x.IsEmergency),
            Announcements = latestAnnouncements
        };

        return Ok(dto);
    }
 
}

public class PlayAudioDto
{
    public int AnnouncementId { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
}