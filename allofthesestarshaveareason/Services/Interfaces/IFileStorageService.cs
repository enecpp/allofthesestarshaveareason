namespace allofthesestarshaveareason.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveUploadedFileAsync(IFormFile file, string? fileName = null, CancellationToken cancellationToken = default);
    
    Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    
    Task<string> CreateTemporaryDirectoryAsync(string? directoryName = null, CancellationToken cancellationToken = default);
    
    Task DeleteDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);
}
