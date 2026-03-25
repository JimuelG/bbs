
using Core.Entities;
using Core.Interfaces;

namespace API.Services;

public class FailoverService(
    IServiceScopeFactory scopeFactory,
    ILogger<FailoverService> logger,
    IHttpClientFactory httpClientFactory,
    IConfiguration config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Failover Service is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var rpiList = await unit.Repository<RPiStatus>().ListAllAsync();
                var rpi = rpiList.FirstOrDefault();

                if (rpi == null || (DateTime.UtcNow - rpi.LastSeen).TotalSeconds > 90)
                {
                    logger.LogWarning("RPi OFFLINE detected. Checking for pending broadcasts...");

                    var pending = await unit.Repository<Announcement>().ListAllAsync();
                    var toFailover = pending.Where(x => !x.IsPlayed && x.ScheduledAt <= DateTime.UtcNow).ToList();

                    foreach (var announcement in toFailover)
                    {
                        logger.LogInformation($"Failing over to SMS: {announcement.Title}");

                        await SendBulkSms(announcement.Title, announcement.Message, unit);

                        announcement.IsPlayed = true;
                        unit.Repository<Announcement>().Update(announcement);
                    }

                    if (toFailover.Any()) await unit.Complete();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured in the FailoverService loop.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SendBulkSms(string title, string message, IUnitOfWork unit)
    {
        var apiKey = config["Semaphore:ApiKey"] ?? string.Empty;
        var senderName = config["Semaphore:SenderName"] ?? "SEMAPHORE";

        var residents = await unit.Repository<Resident>().ListAllAsync();
        var numbers = residents.Where(r => r.AppUser?.IsIdVerified == true).Select(r => r.PhoneNumber).Where(n => !string.IsNullOrEmpty(n)).ToList();

        if (!numbers.Any())
        {
            logger.LogWarning("No verified residents found. SMS cancelled.");

            return;
        }

        var csvNumbers = string.Join(",", numbers);
        var fullMessage = $"[BRGY ALERT] {title}: {message}";

        var client = httpClientFactory.CreateClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("apikey", apiKey),
            new KeyValuePair<string, string>("number", csvNumbers),
            new KeyValuePair<string, string>("message", fullMessage),
            new KeyValuePair<string, string>("sendername", senderName)
        });

        try
        {
            var response = await client.PostAsync("https://api.semaphone.co/api/v4/messages", content);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"Succcessfully broadcasted SMS to {numbers.Count} residents.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                logger.LogError($"Semaphore API Error: {error}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to Semaphore API.");
        }
    }
}
