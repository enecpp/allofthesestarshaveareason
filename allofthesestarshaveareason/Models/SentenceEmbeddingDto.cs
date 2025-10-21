namespace allofthesestarshaveareason.Models;

public class SentenceEmbeddingDto
{
    public required string SegmentId { get; init; }
    public required float[] Vector { get; init; }
}
