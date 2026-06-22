
using API.Hubs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

public class ScheduledAnnouncementService(
    IServiceScopeFactory scopeFactory,
    ILogger<ScheduledAnnouncementService> logger
    ) : BackgroundService
{
    private const string DeviceId = "barangay-rpi-001";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Scheduled Announcement Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<DeviceHub>>();

                var phTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")
                );

                var announcements = await unit.Repository<Announcement>().ListAllAsync();

                var dueAnnouncements = announcements
                    .Where(x =>
                        x.IsActive &&
                        !x.IsPlayed &&
                        !x.ManualTriggerActive &&
                        x.ScheduledAt <= phTime &&
                        !string.IsNullOrWhiteSpace(x.AudioUrl)
                    )
                    .OrderBy(x => x.ScheduledAt)
                    .ToList();
                
                foreach (var announcement in dueAnnouncements)
                {
                    var audioUrl = announcement.AudioUrl;

                    if (audioUrl!.StartsWith("/audio/"))
                    {
                        audioUrl = audioUrl.Replace("/audio/", "/api/audio/");
                    }

                    announcement.ManualTriggerActive = true;
                    unit.Repository<Announcement>().Update(announcement);

                    await unit.Complete();

                    await hubContext.Clients.Group(DeviceId).SendAsync("PlayAudio", new
                    {
                        audioUrl,
                        announcementId = announcement.Id
                    }, stoppingToken);

                    logger.LogInformation(
                        "Scheduled announcement sent to RPi. Id: {AnnouncementId}, Title: {Title}",
                        announcement.Id,
                        announcement.Title
                    );

                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while checking scheduleddd announcements.");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}