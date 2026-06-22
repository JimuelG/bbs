using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;
public class DeviceHub(IUnitOfWork unit) : Hub
{
    public async Task RegisterDevice(string deviceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
        await Clients.Caller.SendAsync("Registered", new
        {
            deviceId,
            status = "Connected",
            connectedAt = DateTime.UtcNow
        });

        await Clients.Group("admins").SendAsync("DeviceStatusChanged", new
        {
           deviceId,
           status = "Online",
           updatedAt = DateTime.UtcNow 
        });
    }

    public async Task RegisterAdmin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
    }

    public async Task SendDeviceStatus(string deviceId, string status)
    {
        await Clients.Group("admins").SendAsync("DeviceStatusChanged", new
        {
           deviceId,
           status,
           updatedAt = DateTime.UtcNow
        });
    }

    public async Task PlaybackCompleted(int announcementId, string deviceId)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(announcementId);

        if (announcement != null)
        {
            announcement.IsPlayed = true;
            announcement.ManualTriggerActive = false;

            unit.Repository<Announcement>().Update(announcement);
            await unit.Complete();
        }

        await Clients.Group("admins").SendAsync("PlaybackCompleted", new
        {
           announcementId,
           deviceId,
           status = "Completed",
           completedAt = DateTime.UtcNow 
        });
    }

    public async Task PlaybackFailed(int announcementId, string deviceId, string error)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(announcementId);

        if (announcement != null)
        {
            announcement.ManualTriggerActive = false;
            announcement.UpdatedAt = DateTime.UtcNow;

            unit.Repository<Announcement>().Update(announcement);
            await unit.Complete();
        }
        
        await Clients.Group("admin").SendAsync("PlaybackFailed", new
        {
           announcementId,
           deviceId,
           status = "Failed",
           error,
           failedAt = DateTime.UtcNow 
        });
    }

}