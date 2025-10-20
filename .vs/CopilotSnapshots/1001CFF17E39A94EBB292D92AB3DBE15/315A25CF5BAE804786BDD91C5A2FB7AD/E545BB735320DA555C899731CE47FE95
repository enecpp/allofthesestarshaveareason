using allofthesestarshaveareason.Models;

namespace allofthesestarshaveareason.Services.Interfaces;

public class SentenceEmbedding
{
    public required string SegmentId { get; init; }
    public required float[] Vector { get; init; }
}

public interface ITextAnalysisService
{
    Task<List<SentenceEmbedding>> GenerateEmbeddingsAsync(List<TranscriptSegment> segments);
    List<TranscriptSegment> FindSimilarSentences(string query, List<TranscriptSegment> allSegments, List<SentenceEmbedding> allEmbeddings);
}
