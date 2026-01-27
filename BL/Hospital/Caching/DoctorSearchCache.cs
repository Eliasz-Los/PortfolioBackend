using System.Text.Json;
using BL.hospital.dto;
using Microsoft.Extensions.Caching.Distributed;

namespace BL.hospital.Caching;

public interface IDoctorSearchCache
{
    Task<IReadOnlyList<DoctorDto>?> TryGet(string? term, CancellationToken cancellationToken = default);
    Task Set(string? term, IReadOnlyList<DoctorDto> doctors, TimeSpan ttl, CancellationToken cancellationToken = default);
}

public class DoctorSearchCache : IDoctorSearchCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;

    public DoctorSearchCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<IReadOnlyList<DoctorDto>?> TryGet(string? term, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(term);
        if (key is null)
        {
            return null;
        }

        var cached = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(cached))
        {
            return null;
        }

        return JsonSerializer.Deserialize<IReadOnlyList<DoctorDto>>(cached, JsonOptions);
    }

    public async Task Set(string? term, IReadOnlyList<DoctorDto> doctors, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(term);
        if (key is null)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(doctors, JsonOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(key, payload, options, cancellationToken);
    }

    private static string? BuildKey(string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return null;

        var normalized = string.Join(' ', 
            term
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToLowerInvariant();

        return $"doctor-search:v1:{normalized}";
    }
}
