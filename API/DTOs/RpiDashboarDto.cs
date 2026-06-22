namespace API.DTOs;
public class RpiDashboardDto
{
    public int TotalRegisteredResidents { get; set; }
    public int TotalAnnouncements { get; set; }
    public int PlayedAnnouncements { get; set; }
    public int PendingAnnouncements { get; set; }
    public int ScheduledAnnouncements { get; set; }
    public int Emergencies { get; set; }

    public IReadOnlyList<RpiAnnouncementDto> Announcements { get; set; } = new List<RpiAnnouncementDto>();
}

public class RpiAnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public bool IsPlayed { get; set; }
    public bool IsEmergency { get; set; }
}