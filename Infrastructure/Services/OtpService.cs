using System.Collections.Concurrent;
using Infrastructure.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public class OtpData
{
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
}
public class OtpService(IServiceScopeFactory scopeFactory) : IOtpService
{
    private static readonly ConcurrentDictionary<string, OtpData> otpStore = new();
    private const int OtpDurationMinutes = 5;
    private const int OtpLength = 6;

    public async Task GenerateAndSendOtpAsync(string email)
    {
        var random = new Random();
        string code = random.Next((int)Math.Pow(10, OtpLength - 1), (int)Math.Pow(10, OtpLength)).ToString();

        var otpData = new OtpData
        {
            Code = code,
            ExpiryTime = DateTime.UtcNow.AddMinutes(OtpDurationMinutes)
        };

        otpStore.AddOrUpdate(email, otpData, (key, existingVal) => otpData);

        string subject = "UgnayBarangay: Forget Password Verication Code";
        string body = BuildForgotPasswordEmailTemplate(code);

        using var scope = scopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        await emailService.SendEmailAsync(email, subject, body);
    }

    public Task InvalidateOptAsync(string email)
    {
        otpStore.TryRemove(email, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ValidateOtpAsync(string email, string otp)
    {
        if (!otpStore.TryGetValue(email, out var storedData))
        {
            return Task.FromResult(false);
        }

        if (storedData.ExpiryTime < DateTime.UtcNow)
        {
            otpStore.TryRemove(email, out _);
            return Task.FromResult(false);
        }

        if (storedData.Code == otp)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private static string BuildForgotPasswordEmailTemplate(string otp)
    {
        return $"""
        <div style="font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden;">
            <div style="background-color: #1e293b; padding: 24px; text-align: center;">
                <h1 style="color: #ffffff; margin: 0;">UGNAYBARANGAY</h1>
                <p style="color: #94a3b8; font-size: 13px;">Barangay Broadcasting & Support System</p>
            </div>

            <div style="padding: 32px;">
                <h2>Magandang Araw</h2>

                <p>
                    May natanggap kaming kahilingan upang baguhin ang iyong password.
                    Gamitin ang verification code sa ibaba:
                </p>

                <div style="background: #fef3c7; border: 1px solid #fde68a; padding: 16px; text-align: center; border-radius: 8px; margin: 24px 0;">
                    <p style="font-size: 24px; font-weight: bold; letter-spacing: 6px; margin: 0;">
                        {otp}
                    </p>
                    <p style="font-size: 12px; margin-top: 8px; color: #92400e;">
                        Valid for 5 minutes
                    </p>
                </div>

                <p>
                    Kung hindi ikaw ang humiling nito, maaari mong balewalain ang email na ito.
                </p>
            </div>

            <div style="background-color: #f1f5f9; padding: 16px; text-align: center;">
                <p style="font-size: 12px; color: #64748b;">
                    This is an automated message. Please do not reply.
                </p>
            </div>
        </div>
        """;
    }
}