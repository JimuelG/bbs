using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AnnouncementRepository(AppDbContext context) : IAnnouncementRepository
{
    public async Task AddAsync(Announcement announcement)
    {
        await context.Announcements.AddAsync(announcement);
    }

    public async Task<IReadOnlyList<Announcement>> GetActiveAsync()
    {
        return await context.Announcements
            .Where(a => a.IsActive && !a.IsPlayed)
            .ToListAsync();
    }

    public async Task<Announcement?> GetById(int id)
    {
        return await context.Announcements
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Announcement?> GetLatestAsync()
    {
        return await context.Announcements
            .Where(a => a.IsActive && !a.IsPlayed && a.ScheduledAt <= DateTime.UtcNow)
            .OrderByDescending(a => a.IsEmergency)
            .ThenBy(a => a.ScheduledAt)
            .FirstOrDefaultAsync();
    }

    public void Update(Announcement announcement)
    {
        context.Announcements.Update(announcement);
    }
}
