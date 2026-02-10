namespace API.DTOs;

public class AnnouncementResponseDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public bool IsEmergency { get; set; }
    public DateTime ScheduledAt { get; set; }
}
