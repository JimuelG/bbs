using System.Security.Cryptography;
using System.Text;
using Core.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class GoogleTtsService : ITtsService
{
    private readonly string _audioFolder;
    private readonly GoogleCredential _googleCredential;
    public GoogleTtsService(IConfiguration config)
    {
        _audioFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio");
        
        Directory.CreateDirectory(_audioFolder);

        var credentialsPath = 
            Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")
            ??config["GoogleCloud:CredentialsPath"]
            ?? throw new Exception("GoogleCloud:CredentialsPath is missing.");

        if (!File.Exists(credentialsPath))
        {
            throw new FileNotFoundException(
                $"Google credentials file not found: {credentialsPath}"
            );
        }

        using var stream = File.OpenRead(credentialsPath);

        var serviceAccountCredential =
            ServiceAccountCredential.FromServiceAccountData(stream);

        _googleCredential = GoogleCredential
            .FromServiceAccountCredential(serviceAccountCredential)
            .CreateScoped(TextToSpeechClient.DefaultScopes);
    }

    public async Task<string> GenerateSpeechAsync(string text, bool isEmergency, string languageCode = "fil-PH")
    {
        var voiceMapping = new Dictionary<string, string>
        {
            { "fil-PH", "fil-PH-Wavenet-A"},
            { "en-US", "en-US-Neural2-F"}
        };

        var selectedVoice = voiceMapping.ContainsKey(languageCode)
                            ? voiceMapping[languageCode]
                            : "fil-PH-Wavenet-A";

        var speakingRate = isEmergency ? 1.0 : .8;

        var hasInput = $"{text}-{languageCode}-{selectedVoice}-{speakingRate}";
        var fileName = GenerateHash(hasInput) + ".mp3";
        var filePath = Path.Combine(_audioFolder, fileName);

        if (File.Exists(filePath))
        {
            return $"/audio/{fileName}";
        }

        var client = await new TextToSpeechClientBuilder
        {
            GoogleCredential = _googleCredential
        }.BuildAsync();

        var input = new SynthesisInput
        {
            Text = text
        };

        var voice = new VoiceSelectionParams
        {
            LanguageCode = languageCode,
            Name = selectedVoice,
            SsmlGender = SsmlVoiceGender.Female
        };

        var audioConfig = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3,
            SpeakingRate = speakingRate
        };

        var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig);

        await File.WriteAllBytesAsync(filePath, response.AudioContent.ToByteArray());

        return $"/audio/{fileName}";

    }

    public static string GenerateHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
