using allofthesestarshaveareason.Services.Interfaces;
using allofthesestarshaveareason.Models;
using Whisper.net;
using Whisper.net.Ggml;

namespace allofthesestarshaveareason.Services.Implementations;

public class WhisperTranscriptService : ITranscriptService
{
    private readonly ILogger<WhisperTranscriptService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _modelPath;
    private readonly SemaphoreSlim _modelLock = new(1, 1);

    public WhisperTranscriptService(
        ILogger<WhisperTranscriptService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _modelPath = configuration["Whisper:ModelPath"] ?? "models/ggml-base.bin";
    }

    public async Task<bool> EnsureModelReadyAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(_modelPath))
        {
            return true;
        }

        await _modelLock.WaitAsync(cancellationToken);
        try
        {
            if (File.Exists(_modelPath))
            {
                return true;
            }

            _logger.LogInformation("Whisper model not found at {ModelPath}. Downloading...", _modelPath);
            await DownloadModelAsync(cancellationToken);
            _logger.LogInformation("Whisper model downloaded successfully");
            return true;
        }
        finally
        {
            _modelLock.Release();
        }
    }

    public async Task<IReadOnlyList<TranscriptSegment>> GenerateTranscriptAsync(
        string audioPath,
        string? language = null,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureModelReadyAsync(cancellationToken);

        _logger.LogInformation("Generating transcript from {AudioPath}", audioPath);

        var segments = new List<TranscriptSegment>();

        try
        {
            using var whisperFactory = WhisperFactory.FromPath(_modelPath);
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage(language ?? "auto")
                .Build();

            using var fileStream = File.OpenRead(audioPath);
            
            var processedSegments = 0;
            await foreach (var result in processor.ProcessAsync(fileStream, cancellationToken))
            {
                var segment = new TranscriptSegment
                {
                    StartTime = result.Start.TotalSeconds,
                    EndTime = result.End.TotalSeconds,
                    Text = result.Text.Trim(),
                    Speaker = "Narrator"
                };

                segments.Add(segment);
                processedSegments++;

                progress?.Report(processedSegments);

                _logger.LogDebug("Segment [{Start} - {End}]: {Text}",
                    result.Start.ToString(@"mm\:ss"),
                    result.End.ToString(@"mm\:ss"),
                    segment.Text);
            }

            _logger.LogInformation("Transcript generated with {Count} segments", segments.Count);
            return segments.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript from {AudioPath}", audioPath);
            throw new InvalidOperationException($"Transkript oluþturma hatasý: {ex.Message}", ex);
        }
    }

    private async Task DownloadModelAsync(CancellationToken cancellationToken)
    {
        var modelDirectory = Path.GetDirectoryName(_modelPath);
        if (!string.IsNullOrEmpty(modelDirectory) && !Directory.Exists(modelDirectory))
        {
            Directory.CreateDirectory(modelDirectory);
        }

        var httpClient = _httpClientFactory.CreateClient();
        var downloader = new WhisperGgmlDownloader(httpClient);
        
        using var modelStream = await downloader.GetGgmlModelAsync(GgmlType.Base, cancellationToken: cancellationToken);
        using var fileWriter = File.OpenWrite(_modelPath);
        
        await modelStream.CopyToAsync(fileWriter, cancellationToken);
    }
}
