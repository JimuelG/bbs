namespace API.DTOs;

public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public bool IsEmergency { get; set; }
}
