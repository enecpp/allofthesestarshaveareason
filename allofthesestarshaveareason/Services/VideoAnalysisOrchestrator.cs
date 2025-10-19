using allofthesestarshaveareason.Services.Interfaces;
using allofthesestarshaveareason.Models;

namespace allofthesestarshaveareason.Services;

public class VideoAnalysisOrchestrator : IAnalysisService
{
    private readonly ILogger<VideoAnalysisOrchestrator> _logger;
    private readonly IJobRepository _jobRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly IFFmpegService _ffmpegService;
    private readonly ITranscriptService _transcriptService;
    private readonly ISceneDetectionService _sceneDetectionService;

    public VideoAnalysisOrchestrator(
        ILogger<VideoAnalysisOrchestrator> logger,
        IJobRepository jobRepository,
        IFileStorageService fileStorage,
        IFFmpegService ffmpegService,
        ITranscriptService transcriptService,
        ISceneDetectionService sceneDetectionService)
    {
        _logger = logger;
        _jobRepository = jobRepository;
        _fileStorage = fileStorage;
        _ffmpegService = ffmpegService;
        _transcriptService = transcriptService;
        _sceneDetectionService = sceneDetectionService;
    }

    public async Task<string> StartAnalysisJobAsync(IFormFile file)
    {
        if (!_ffmpegService.IsFFmpegAvailable())
        {
            throw new InvalidOperationException(
                "FFmpeg bulunamadý! Lütfen FFmpeg'i kurun ve sistem PATH'ine ekleyin.");
        }

        var jobId = await _jobRepository.CreateJobAsync();
        _logger.LogInformation("Starting analysis job: {JobId} for file: {FileName}", jobId, file.FileName);

        var videoPath = await _fileStorage.SaveUploadedFileAsync(file, $"{jobId}_{file.FileName}");

        _ = Task.Run(async () => await ProcessVideoAsync(jobId, videoPath));

        return jobId;
    }

    public async Task<AnalysisStatus> GetAnalysisStatusAsync(string jobId)
    {
        var status = await _jobRepository.GetJobStatusAsync(jobId);
        return status ?? new AnalysisStatus { Status = "Bulunamadý", Progress = 0 };
    }

    private async Task ProcessVideoAsync(string jobId, string videoPath)
    {
        string? audioPath = null;
        string? framesDirectory = null;

        try
        {
            await _jobRepository.UpdateJobStatusAsync(jobId, "Ses ayýklanýyor...", 10);
            _logger.LogInformation("Job {JobId}: Extracting audio", jobId);

            audioPath = Path.Combine("uploads", $"{jobId}.wav");
            await _ffmpegService.ExtractAudioAsync(videoPath, audioPath);

            await _jobRepository.UpdateJobStatusAsync(jobId, "Transkript oluþturuluyor...", 30);
            _logger.LogInformation("Job {JobId}: Generating transcript", jobId);

            var transcriptProgress = new Progress<int>(count =>
            {
                _logger.LogDebug("Job {JobId}: Processed {Count} transcript segments", jobId, count);
            });

            var transcript = await _transcriptService.GenerateTranscriptAsync(
                audioPath,
                language: "auto",
                progress: transcriptProgress);

            _logger.LogInformation("Job {JobId}: Transcript completed with {Count} segments",
                jobId, transcript.Count);

            await _jobRepository.UpdateJobStatusAsync(jobId, "Sahneler tespit ediliyor...", 60);
            _logger.LogInformation("Job {JobId}: Detecting scenes", jobId);

            framesDirectory = await _fileStorage.CreateTemporaryDirectoryAsync(jobId);
            var frames = await _ffmpegService.ExtractFramesAsync(videoPath, framesDirectory, framesPerSecond: 1);

            var sceneProgress = new Progress<int>(processed =>
            {
                var percentage = 60 + (processed * 20 / frames.Count);
                _jobRepository.UpdateJobStatusAsync(jobId, "Sahneler tespit ediliyor...", percentage).Wait();
            });

            var scenes = await _sceneDetectionService.DetectScenesAsync(
                frames,
                threshold: 0.7,
                progress: sceneProgress);

            _logger.LogInformation("Job {JobId}: Scene detection completed with {Count} scenes",
                jobId, scenes.Count);

            await _jobRepository.UpdateJobStatusAsync(jobId, "Sonuçlar kaydediliyor...", 90);
            _logger.LogInformation("Job {JobId}: Saving results", jobId);

            await _jobRepository.SaveJobResultsAsync(jobId, (transcript.ToList(), scenes.ToList()));

            await _jobRepository.CompleteJobAsync(jobId, resultId: 123);
            _logger.LogInformation("Job {JobId}: Processing completed successfully", jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobId}: Error during processing", jobId);
            await _jobRepository.FailJobAsync(jobId, ex.Message);
        }
        finally
        {
            await CleanupResourcesAsync(jobId, audioPath, framesDirectory);
        }
    }

    private async Task CleanupResourcesAsync(string jobId, string? audioPath, string? framesDirectory)
    {
        if (!string.IsNullOrEmpty(audioPath))
        {
            try
            {
                await _fileStorage.DeleteFileAsync(audioPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Job {JobId}: Failed to delete audio file: {AudioPath}",
                    jobId, audioPath);
            }
        }

        if (!string.IsNullOrEmpty(framesDirectory))
        {
            try
            {
                await _fileStorage.DeleteDirectoryAsync(framesDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Job {JobId}: Failed to delete frames directory: {FramesDirectory}",
                    jobId, framesDirectory);
            }
        }
    }
}
