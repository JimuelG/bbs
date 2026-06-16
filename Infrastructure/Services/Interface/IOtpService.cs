namespace Infrastructure.Services.Interface
{
    public interface IOtpService
    {
        Task GenerateAndSendOtpAsync(string email);
        Task<bool> ValidateOtpAsync(string email, string otp);
        Task InvalidateOptAsync(string email);
    }
}