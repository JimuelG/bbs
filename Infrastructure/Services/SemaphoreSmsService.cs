using Core.Models;
using Infrastructure.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class SemaphoreSmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly SemaphoreOptions _options;
    private readonly ILogger<SemaphoreSmsService> _logger;

    public SemaphoreSmsService(
        HttpClient httpClient,
        IOptions<SemaphoreOptions> options,
        ILogger<SemaphoreSmsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendAnnouncementAsync(IEnumerable<string> phoneNumbers, string message)
    {
        var validNumbers = phoneNumbers
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .ToList();

        if (!validNumbers.Any())
        {
            _logger.LogWarning("SMS Announcement aborted: No valid phone numbers provided.");
            return false;
        }

        string concatenatedNumber = string.Join(",", validNumbers);

        var parameters = new Dictionary<string, string>
        {
            { "apikey", _options.ApiKey },
            { "number", concatenatedNumber },
            { "message", message },
            { "sendername", _options.SenderName }
        };

        try
        {
            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync("https://api.semaphore.co/api/v4/messages", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS Announcement successfully broadcasted to {Count} residents.", validNumbers.Count);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Semaphore API error. Status: {Status}, Response: {Response}", response.StatusCode, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected exception occured while trying to send SMS via Semaphore");
            return false;
        }
    }
}
