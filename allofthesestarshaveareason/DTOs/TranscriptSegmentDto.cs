namespace allofthosestarshaveareason.DTOs
{
    public class TranscriptSegmentDto
    {
        public int Id { get; set; }
        public string Speaker { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }
}