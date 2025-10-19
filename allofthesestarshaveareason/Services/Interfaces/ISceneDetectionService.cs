using allofthesestarshaveareason.Models;

namespace allofthesestarshaveareason.Services.Interfaces;

public interface ISceneDetectionService
{
    Task<IReadOnlyList<Scene>> DetectScenesAsync(
        IReadOnlyList<string> framePaths, 
        double threshold = 0.7,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
}
