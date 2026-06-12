namespace Infrastructure.Services.Interface;

public interface ISmsService
{
    Task<bool> SendAnnouncementAsync(IEnumerable<string> phoneNumbers, string message);
}
