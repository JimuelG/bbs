namespace API.DTOs;

public class AnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public bool IsEmergency { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime ScheduledAt { get; set; }
}
