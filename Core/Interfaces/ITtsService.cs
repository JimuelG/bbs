namespace Core.Interfaces;

public interface ITtsService
{
    Task<string> GenerateSpeechAsync(string text, bool isEmergency, string languageCode);
}
