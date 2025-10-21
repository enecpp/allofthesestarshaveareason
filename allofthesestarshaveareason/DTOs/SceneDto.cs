namespace allofthosestarshaveareason.DTOs
{
    public class SceneDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }
}