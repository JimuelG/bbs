namespace API.DTOs;

public class DisplayAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsEmergency { get; set; }
}
