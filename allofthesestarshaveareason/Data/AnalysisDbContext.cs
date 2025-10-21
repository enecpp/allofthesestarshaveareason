using allofthesestarshaveareason.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json; 

namespace allofthesestarshaveareason.Data
{
    public class AnalysisDbContext : DbContext
    {
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) { }

        public DbSet<AnalysisStatus> JobStatuses { get; set; }
        public DbSet<VideoAnalysisResult> Results { get; set; }
        public DbSet<TranscriptSegment> Segments { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SentenceEmbedding> Embeddings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalysisStatus>()
                .HasOne(a => a.Result)
                .WithOne(r => r.AnalysisStatus)
                .HasForeignKey<VideoAnalysisResult>(r => r.AnalysisStatusId);

            modelBuilder.Entity<VideoAnalysisResult>()
                .HasMany(r => r.Transcript)
                .WithOne(t => t.VideoAnalysisResult)
                .HasForeignKey(t => t.VideoAnalysisResultId);

            modelBuilder.Entity<VideoAnalysisResult>()
                .HasMany(r => r.Scenes)
                .WithOne(s => s.VideoAnalysisResult)
                .HasForeignKey(s => s.VideoAnalysisResultId);

            modelBuilder.Entity<TranscriptSegment>()
                .HasOne(t => t.Embedding)
                .WithOne(e => e.TranscriptSegment)
                .HasForeignKey<SentenceEmbedding>(e => e.TranscriptSegmentId);

            modelBuilder.Entity<SentenceEmbedding>()
                .Property(e => e.Vector)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<float[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<float>()
                );
        }
    }
}