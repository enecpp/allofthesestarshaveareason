using allofthesestarshaveareason.Models;
using allofthesestarshaveareason.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using TranscriptSegment = allofthesestarshaveareason.Models.TranscriptSegment;

namespace allofthesestarshaveareason.Services.Implementations;

public class OnnxTextAnalysisService : ITextAnalysisService, IDisposable
{
    private readonly InferenceSession _session;
    private readonly Tokenizer _tokenizer;
    private readonly ILogger<OnnxTextAnalysisService> _logger;
    private bool _disposed;

    public OnnxTextAnalysisService(IWebHostEnvironment env, ILogger<OnnxTextAnalysisService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(env);

        try
        {
            var modelPath = Path.Combine(env.ContentRootPath, "wwwroot", "ml-models", "model.onnx");
            var vocabPath = Path.Combine(env.ContentRootPath, "wwwroot", "ml-models", "vocab.txt");

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException($"ONNX model file not found at: {modelPath}");
            }

            if (!File.Exists(vocabPath))
            {
                throw new FileNotFoundException($"Vocabulary file not found at: {vocabPath}");
            }

            _session = new InferenceSession(modelPath);
            using var stream = File.OpenRead(vocabPath);
            _tokenizer = BertTokenizer.Create(stream);
            
            _logger.LogInformation("ONNX model and tokenizer loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize OnnxTextAnalysisService");
            throw;
        }
    }

    public async Task<List<SentenceEmbeddingDto>> GenerateEmbeddingsAsync(List<TranscriptSegment> segments)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OnnxTextAnalysisService));
        }
        ArgumentNullException.ThrowIfNull(segments);

        if (segments.Count == 0)
        {
            _logger.LogWarning("No segments provided for embedding generation");
            return new List<SentenceEmbeddingDto>();
        }

        return await Task.Run(() =>
        {
            try
            {
                var embeddings = new List<SentenceEmbeddingDto>();

                foreach (var segment in segments)
                {
                    string text = segment.Text ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    var embedding = GenerateEmbeddingForSingleText(text, $"segment_{segments.IndexOf(segment)}");
                    if (embedding != null)
                    {
                        embeddings.Add(embedding);
                    }
                }

                _logger.LogInformation("Generated {Count} embeddings successfully", embeddings.Count);
                return embeddings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embeddings");
                throw new InvalidOperationException("Failed to generate embeddings", ex);
            }
        }).ConfigureAwait(false);
    }

    public List<TranscriptSegment> FindSimilarSentences(
        string query, 
        List<TranscriptSegment> allSegments, 
        List<SentenceEmbeddingDto> allEmbeddings)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OnnxTextAnalysisService));
        }
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(allSegments);
        ArgumentNullException.ThrowIfNull(allEmbeddings);

        if (string.IsNullOrWhiteSpace(query))
        {
            _logger.LogWarning("Empty query provided for similarity search");
            return new List<TranscriptSegment>();
        }

        if (allSegments.Count == 0 || allEmbeddings.Count == 0)
        {
            return new List<TranscriptSegment>();
        }

        try
        {
            var queryEmbedding = GenerateEmbeddingsForSingleText(query);
            if (queryEmbedding == null || queryEmbedding.Length == 0)
            {
                _logger.LogWarning("Failed to generate embedding for query");
                return new List<TranscriptSegment>();
            }

            var scoredSegments = new List<(TranscriptSegment segment, double score)>();
            int segmentCount = Math.Min(allSegments.Count, allEmbeddings.Count);
            
            for (int i = 0; i < segmentCount; i++)
            {
                double score = CosineSimilarity(queryEmbedding, allEmbeddings[i].Vector);
                scoredSegments.Add((allSegments[i], score));
            }

            var results = scoredSegments
                .Where(s => !double.IsNaN(s.score))
                .OrderByDescending(s => s.score)
                .Take(5)
                .Select(s => s.segment)
                .ToList();

            _logger.LogInformation("Found {Count} similar segments for query", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar sentences");
            return new List<TranscriptSegment>();
        }
    }

    private SentenceEmbeddingDto? GenerateEmbeddingForSingleText(string text, string segmentId)
    {
        try
        {
            var vector = GenerateEmbeddingsForSingleText(text);
            if (vector == null || vector.Length == 0)
            {
                return null;
            }

            return new SentenceEmbeddingDto
            {
                SegmentId = segmentId,
                Vector = vector
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sentence embedding");
            return null;
        }
    }

    private float[]? GenerateEmbeddingsForSingleText(string text)
    {
        try
        {
            var encoded = _tokenizer.EncodeToTokens(text, out _);
            
            if (encoded == null || encoded.Count == 0)
            {
                return null;
            }

            var ids = encoded.Select(t => t.Id).ToList();
            int tokenCount = ids.Count;
            var attentionMask = Enumerable.Repeat(1, tokenCount).ToList();
            var tokenTypeIds = Enumerable.Repeat(0, tokenCount).ToList();

            var inputIds = ConvertToTensor(ids);
            var attentionMaskTensor = ConvertToTensor(attentionMask);
            var tokenTypeIdsTensor = ConvertToTensor(tokenTypeIds);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIds),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
                NamedOnnxValue.CreateFromTensor("token_type_ids", tokenTypeIdsTensor)
            };

            using var results = _session.Run(inputs);
            var lastHiddenState = results.First().AsTensor<float>();
            return MeanPooling(lastHiddenState, attentionMask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text");
            return null;
        }
    }

    private static DenseTensor<long> ConvertToTensor(List<int> ids)
    {
        if (ids.Count == 0)
        {
            return new DenseTensor<long>(new long[] { 0 }, new[] { 1, 1 });
        }
        var longArray = ids.Select(x => (long)x).ToArray();
        return new DenseTensor<long>(longArray, new[] { 1, longArray.Length });
    }

    private static float[] MeanPooling(Tensor<float>? tokenEmbeddings, List<int> attentionMask)
    {
        if (tokenEmbeddings == null || attentionMask.Count == 0)
        {
            return Array.Empty<float>();
        }

        int tokenCount = attentionMask.Count;
        long embeddingDim = tokenEmbeddings.Length / tokenCount;
        
        if (embeddingDim <= 0)
        {
            return Array.Empty<float>();
        }

        var pooled = new float[embeddingDim];
        int validTokenCount = 0;

        for (int i = 0; i < tokenCount; i++)
        {
            if (attentionMask[i] == 1)
            {
                validTokenCount++;
                for (int j = 0; j < embeddingDim; j++)
                {
                    long index = i * embeddingDim + j;
                    if (index < tokenEmbeddings.Length)
                    {
                        pooled[j] += tokenEmbeddings.GetValue((int)index);
                    }
                }
            }
        }

        if (validTokenCount == 0)
        {
            return pooled;
        }

        for (int j = 0; j < embeddingDim; j++)
        {
            pooled[j] /= validTokenCount;
        }

        return pooled;
    }

    private static double CosineSimilarity(float[] vec1, float[] vec2)
    {
        ArgumentNullException.ThrowIfNull(vec1);
        ArgumentNullException.ThrowIfNull(vec2);

        if (vec1.Length != vec2.Length)
        {
            throw new ArgumentException("Vectors must have the same length");
        }

        if (vec1.Length == 0)
        {
            return 0.0;
        }

        double dotProduct = 0.0;
        double mag1 = 0.0;
        double mag2 = 0.0;

        for (int i = 0; i < vec1.Length; i++)
        {
            dotProduct += vec1[i] * vec2[i];
            mag1 += vec1[i] * vec1[i];
            mag2 += vec2[i] * vec2[i];
        }

        double magnitude = Math.Sqrt(mag1) * Math.Sqrt(mag2);
        return magnitude == 0 ? 0.0 : dotProduct / magnitude;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _session?.Dispose();
        _disposed = true;
        
        GC.SuppressFinalize(this);
        _logger.LogInformation("OnnxTextAnalysisService disposed");
    }
}
