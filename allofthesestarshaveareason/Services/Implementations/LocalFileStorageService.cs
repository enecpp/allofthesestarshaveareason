using allofthesestarshaveareason.Services.Interfaces;

namespace allofthesestarshaveareason.Services.Implementations;

public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _uploadDirectory = "uploads";

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        EnsureUploadDirectoryExists();
    }

    public async Task<string> SaveUploadedFileAsync(
        IFormFile file,
        string? fileName = null,
        CancellationToken cancellationToken = default)
    {
        fileName ??= $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_uploadDirectory, fileName);

        _logger.LogInformation("Saving uploaded file to {FilePath}", filePath);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        _logger.LogInformation("File saved successfully: {FilePath}", filePath);
        return filePath;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<string> CreateTemporaryDirectoryAsync(
        string? directoryName = null,
        CancellationToken cancellationToken = default)
    {
        directoryName ??= Guid.NewGuid().ToString();
        var directoryPath = Path.Combine(_uploadDirectory, directoryName);

        Directory.CreateDirectory(directoryPath);
        _logger.LogInformation("Temporary directory created: {DirectoryPath}", directoryPath);

        return Task.FromResult(directoryPath);
    }

    public Task DeleteDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
                _logger.LogInformation("Directory deleted: {DirectoryPath}", directoryPath);
            }
            else
            {
                _logger.LogWarning("Directory not found for deletion: {DirectoryPath}", directoryPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting directory: {DirectoryPath}", directoryPath);
            throw;
        }

        return Task.CompletedTask;
    }

    private void EnsureUploadDirectoryExists()
    {
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
            _logger.LogInformation("Upload directory created: {UploadDirectory}", _uploadDirectory);
        }
    }
}
