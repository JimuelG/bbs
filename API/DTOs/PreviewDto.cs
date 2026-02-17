namespace API.DTOs;

public class PreviewDto
{
    public string Message { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public bool IsEmergency { get; set; }
}
