using System.Threading.RateLimiting;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Interfaces;
using SAP_AdresToLatLong.Models;
using Polly;
using Polly.RateLimiting;

namespace SAP_AdresToLatLong.Services;

public class GeocodeService : IGeocodeService
{
    private readonly GoogleApiClient _client;
    private readonly ResiliencePipeline _pipeline;
    private readonly Random _random = new();

    public GeocodeService(GoogleApiClient client)
    {
        _client = client;
        _pipeline = new ResiliencePipelineBuilder()
            .AddRateLimiter(new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                TokenLimit = 3000,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 3000,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }))
            .Build();
    }

    public async Task<List<PostGeocodeData?>> GetGeocodeDataBatchAsync(
        IEnumerable<SAPData> batch,
        ApplicationDbContext context)
    {
        List<PostGeocodeData?> results = new();

        foreach (var sapData in batch)
        {
            var jitter = _random.Next(10, 100);
            await Task.Delay(jitter);
            try
            {
                var result = await _pipeline.ExecuteAsync(
                    async token => await _client.GetGeocodeDataAsync(sapData, context));

                results.Add(result);
            }
            catch (RateLimiterRejectedException)
            {
                results.Add(null);
            }
            
        }

        return results;
    }
}