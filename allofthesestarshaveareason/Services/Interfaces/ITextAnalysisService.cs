using allofthesestarshaveareason.Models;

namespace allofthesestarshaveareason.Services.Interfaces;

public interface ITextAnalysisService
{
    Task<List<SentenceEmbeddingDto>> GenerateEmbeddingsAsync(List<TranscriptSegment> segments);
    List<TranscriptSegment> FindSimilarSentences(string query, List<TranscriptSegment> allSegments, List<SentenceEmbeddingDto> allEmbeddings);
}
