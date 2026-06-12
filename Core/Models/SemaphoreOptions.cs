namespace Core.Models;

public class SemaphoreOptions
{
    public const string SectionName = "Semaphore";
    public string ApiKey { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}
