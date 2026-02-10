using System;

namespace Core.Entities;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public bool IsEmergency { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPlayed { get; set; } = false;
}
