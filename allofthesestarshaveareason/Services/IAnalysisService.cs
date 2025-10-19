namespace allofthesestarshaveareason.Services;

public interface IAnalysisService
{
    Task<string> StartAnalysisJobAsync(IFormFile file);
    
    Task<AnalysisStatus> GetAnalysisStatusAsync(string jobId);
}
