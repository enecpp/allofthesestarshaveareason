namespace allofthesestarshaveareason.Configuration;

public class WhisperConfiguration
{
    public const string SectionName = "Whisper";
    
    public string ModelPath { get; set; } = "models/ggml-base.bin";
    
    public string ModelType { get; set; } = "base";
    
    public string? DefaultLanguage { get; set; }
}

public class FileStorageConfiguration
{
    public const string SectionName = "FileStorage";
    
    public string UploadDirectory { get; set; } = "uploads";
    
    public int MaxFileSizeMB { get; set; } = 500;
    
    public string[] AllowedExtensions { get; set; } = { ".mp4", ".avi", ".mov", ".mkv" };
}

public class SceneDetectionConfiguration
{
    public const string SectionName = "SceneDetection";
    
    public double DefaultThreshold { get; set; } = 0.7;
    
    public int FramesPerSecond { get; set; } = 1;
}

public class FFmpegConfiguration
{
    public const string SectionName = "FFmpeg";
    
    public string? ExecutablePath { get; set; }
    
    public string AudioExtractionArgs { get; set; } = "-vn -acodec pcm_s16le -ar 16000 -ac 1";
}
