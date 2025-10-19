namespace allofthesestarshaveareason.Models;

public class VideoAnalysisResult
{
    public string VideoUrl { get; set; } = string.Empty;
    public List<Scene> Scenes { get; set; } = new();
    public List<TranscriptSegment> Transcript { get; set; } = new();
}

public class Scene
{
    public string Title { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class TranscriptSegment
{
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Speaker { get; set; } = string.Empty;
}
