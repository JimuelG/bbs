using Core.Interfaces;
using Google.Cloud.TextToSpeech.V1;

namespace Infrastructure.Services;

public class GoogleTtsService : ITtsService
{
    private readonly string _audioFolder;
    public GoogleTtsService()
    {
        _audioFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audio");

        if (!Directory.Exists(_audioFolder))
            Directory.CreateDirectory(_audioFolder);
    }

    public async Task<string> GenerateSpeechAsync(string text, bool isEmergency)
    {
        var client = await TextToSpeechClient.CreateAsync();

        var input = new SynthesisInput
        {
            Text = text
        };

        var voice = new VoiceSelectionParams
        {
            LanguageCode = "en-US",
            SsmlGender = SsmlVoiceGender.Female
        };

        var audioConfig = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3,
            SpeakingRate = isEmergency ? 1.2 : 1.0
        };

        var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig);

        var fileName = $"{Guid.NewGuid()}.mp3";
        var filePath = Path.Combine(_audioFolder, fileName);

        await File.WriteAllBytesAsync(filePath, response.AudioContent.ToByteArray());

        return $"/audio/{fileName}";

    }
}
