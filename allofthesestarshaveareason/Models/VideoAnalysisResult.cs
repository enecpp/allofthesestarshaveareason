using allofthesestarshaveareason.Services.Interfaces;
using System.Globalization;

namespace allofthesestarshaveareason.Models;

public class VideoAnalysisResult
{
    public int Id { get; set; }
    public DateTime ProcessedDate { get; set; }
    public Guid AnalysisStatusId { get; set; } 
    public AnalysisStatus AnalysisStatus { get; set; } = null!;
    public string VideoUrl { get; set; } = string.Empty;
    public List<TranscriptSegment> Transcript { get; set; } = new();
    public List<Scene> Scenes { get; set; } = new();
}

public class Scene
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }

    public int VideoAnalysisResultId { get; set; }
    public VideoAnalysisResult VideoAnalysisResult { get; set; } = null!;
}

public class TranscriptSegment
{
    public int Id { get; set; }
    public string Speaker { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double EndTime { get; set; }

    public int VideoAnalysisResultId { get; set; }
    public VideoAnalysisResult VideoAnalysisResult { get; set; } = null!;

    public SentenceEmbedding? Embedding { get; set; }
}

public class AnalysisStatus
{ 
    public Guid Id { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public int? ResultId { get; set; } // EKLENDÝ
    public VideoAnalysisResult? Result { get; set; }
}

public class SentenceEmbedding
{
    public int Id { get; set; }
    public float[] Vector { get; set; } = [];

    public int TranscriptSegmentId { get; set; }
    public TranscriptSegment TranscriptSegment { get; set; } = null!;
}


